using System.Net;
using Discounts.Application.Common.Exceptions;
using Discounts.Application.Common.Models;
using FluentValidation;

namespace Discounts.Api.Middlewares
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public GlobalExceptionHandlerMiddleware(RequestDelegate request, ILogger<GlobalExceptionHandlerMiddleware> logger, IHostEnvironment env)
        {
            _next = request;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex).ConfigureAwait(false);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {

            if (context.Response.HasStarted)
            {
                _logger.LogWarning("Response has already started, cannot handle exception");
                return;
            }

            var response = context.Response;
            response.Clear();
            response.ContentType = "application/json";

            var errorResponse = new ErrorResponse
            {
                TraceId = context.TraceIdentifier,
                Timestamp = DateTime.UtcNow,
                Instance = context.Request.Path
            };

            switch (ex)
            {
                case ValidationException validationException:
                    _logger.LogWarning(ex, "Validation error occurred");
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Status = response.StatusCode;
                    errorResponse.Title = "Validation failed";
                    errorResponse.Detail = "One or more validation errors occurred.";
                    errorResponse.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                    errorResponse.Errors = validationException.Errors
                        .Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
                        .ToList();
                    break;

                case NotFoundException notFoundException:
                    _logger.LogWarning(ex, "Resource not found: {Message}", ex.Message);
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse.Status = response.StatusCode;
                    errorResponse.Title = "Resource not found";
                    errorResponse.Detail = notFoundException.Message;
                    errorResponse.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4";
                    break;

                case UnauthorizedAccessException:
                    _logger.LogWarning(ex, "Unauthorized access attempt");
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    errorResponse.Status = response.StatusCode;
                    errorResponse.Title = "Access denied";
                    errorResponse.Detail = "You are not authorized to perform this action.";
                    errorResponse.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3";
                    break;

                case InvalidOperationException invalidOpException:
                    _logger.LogWarning(ex, "Invalid operation: {Message}", ex.Message);
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Status = response.StatusCode;
                    errorResponse.Title = "Invalid operation";
                    errorResponse.Detail = invalidOpException.Message;
                    errorResponse.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                    break;

                case ArgumentException argumentException:
                    _logger.LogWarning(ex, "Invalid argument: {Message}", ex.Message);
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Status = response.StatusCode;
                    errorResponse.Title = "Invalid argument";
                    errorResponse.Detail = argumentException.Message;
                    errorResponse.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                    break;

                case Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException:
                    _logger.LogWarning(ex, "Concurrency conflict occurred.");
                    response.StatusCode = (int)HttpStatusCode.Conflict;
                    errorResponse.Status = response.StatusCode;
                    errorResponse.Title = "Concurrency conflict";
                    errorResponse.Detail = "The resource was modified by another user. Please reload and try again.";
                    errorResponse.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8";
                    break;

                default:
                    _logger.LogError(ex, "Unhandled exception occurred: {Message}", ex.Message);
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.Status = response.StatusCode;
                    errorResponse.Title = "Internal Server Error";
                    errorResponse.Detail = _env.IsDevelopment()
                        ? ex.Message
                        : "An error occurred while processing your request";
                    errorResponse.Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1";

                    if (_env.IsDevelopment())
                    {
                        errorResponse.Errors = new List<string>
                        {
                            ex.StackTrace ?? string.Empty
                        };
                    }
                    break;
            }

            var jsonResponse = System.Text.Json.JsonSerializer.Serialize(errorResponse, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            });

            await response.WriteAsync(jsonResponse).ConfigureAwait(false);
        }
    }
}
