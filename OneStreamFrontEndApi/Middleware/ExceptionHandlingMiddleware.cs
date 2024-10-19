using System.Net;
using System.Text.Json;

namespace OneStreamFrontEndApi.Middleware
{
    /// <summary>
    /// Middleware to handle exceptions globally in the application pipeline.
    /// Captures exceptions, logs them, and returns appropriate HTTP status codes and error messages.
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionHandlingMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next delegate in the middleware pipeline.</param>
        /// <param name="logger">The logger instance used for logging exceptions.</param>
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Invokes the middleware to process the current HTTP context.
        /// Wraps the execution in a try-catch block to handle any exceptions.
        /// </summary>
        /// <param name="context">The HTTP context of the current request.</param>
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

        /// <summary>
        /// Handles the exception by setting the appropriate HTTP response status code and writing the error message as JSON.
        /// </summary>
        /// <param name="context">The HTTP context of the current request.</param>
        /// <param name="ex">The exception that occurred.</param>
        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            // Map different exception types to HTTP status codes
            var response = context.Response;

            var statusCode = ex switch
            {
                HttpRequestException httpRequestException when
                    httpRequestException.StatusCode == HttpStatusCode.TooManyRequests => HttpStatusCode.TooManyRequests, // Handle 429 Too Many Requests
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
