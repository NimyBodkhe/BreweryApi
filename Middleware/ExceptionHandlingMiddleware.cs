using BreweryApi.Models;
using System.Net;

namespace BreweryApi.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate requestDelegate, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _requestDelegate = requestDelegate;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _requestDelegate(context);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Bad request occured");
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new ApiErrorResponse
                {
                    StatusCode = context.Response.StatusCode,
                    Message = ex.Message,
                });
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "External API error");
                context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new ApiErrorResponse
                {
                    StatusCode = context.Response.StatusCode,
                    Message = ex.Message,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled excpetion occred");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new ApiErrorResponse
                {
                    StatusCode = context.Response.StatusCode,
                    Message = "Something is not working"
                });
            }
        }
    }
}

