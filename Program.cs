using OneStreamFrontEndApi.Middleware;
using OneStreamFrontEndApi.Services;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();


Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/app.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Configure HttpClient with Polly retry policy that includes handling 429 errors
builder.Services.AddHttpClient("PollyClient")
    .AddPolicyHandler((provider, request) =>
    {
        var logger = provider.GetRequiredService<ILogger<Program>>();
        return GetRetryPolicy(logger);
    });

builder.Services.AddTransient<IApiServices, ApiServices>();
builder.Services.AddTransient<IFileServices, FileServices>();
builder.Services.AddMemoryCache(); // Enable in-memory caching
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Enable Swagger for API documentation


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Register the custom exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();


// Define the retry policy with exponential backoff and handling 429 errors
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(ILogger logger)
{
    return HttpPolicyExtensions
        .HandleTransientHttpError() // Handles 5xx and 408 errors
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests) // Handle 429 Too Many Requests
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            onRetry: (outcome, timespan, retryAttempt, context) =>
            {
                // Log retry attempt details with ILogger
                logger.LogWarning($"Retry {retryAttempt} due to {outcome.Result.StatusCode}. Waiting {timespan.TotalSeconds} seconds.");
            });
}