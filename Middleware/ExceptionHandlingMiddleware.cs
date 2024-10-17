using System.Net;
using System.Text.Json;

namespace OneStreamFrontEndApi.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Continue processing the request
                await _next(context);
            }
            catch (Exception ex)
            {
                // Handle the exception and respond with appropriate error message
                _logger.LogError(ex, "An error occurred while processing the request.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            // Map different exception types to HTTP status codes
            var response = context.Response;

            var statusCode = ex switch
            {
                HttpRequestException => HttpStatusCode.BadGateway, // External API error
                UnauthorizedAccessException => HttpStatusCode.Unauthorized, // Unauthorized access
                ArgumentException => HttpStatusCode.BadRequest, // Bad request
                KeyNotFoundException => HttpStatusCode.NotFound, // Not found
                _ => HttpStatusCode.InternalServerError // Generic internal server error
            };

            response.StatusCode = (int)statusCode;

            var result = JsonSerializer.Serialize(new { error = ex.Message, code = statusCode });
            await response.WriteAsync(result);
        }
    }
}
