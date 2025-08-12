using System.Text.Json;
using NLog;

namespace GameService.Middlewares
{
    public class ErrorHandlingMiddleware(RequestDelegate next)
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var (statusCode, message) = exception switch
            {
                ArgumentException _ => (StatusCodes.Status400BadRequest, exception.Message),
                HttpRequestException _ => (StatusCodes.Status503ServiceUnavailable, "Service unavailable. Please try again later."),
                _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again.")
            };

            context.Response.StatusCode = statusCode;
            var result = JsonSerializer.Serialize(new { Error = message });
            return context.Response.WriteAsync(result);
        }
    }
}
