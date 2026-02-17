using Discounts.Application.Common.Exceptions;
using Discounts.Application.Common.Models;
using FluentValidation;
using System.Net;

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
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            // Can't modify response if headers already sent
            if (context.Response.HasStarted)
            {
                _logger.LogWarning("Response has already started, cannot handle exception");
                return;
            }

            var response = context.Response;
            response.Clear(); // Clear any partial response
            response.ContentType = "application/json";

            var errorResponse = new ErrorResponse
            {
                TraceId = context.TraceIdentifier,
                Timestamp = DateTime.UtcNow
            };

            switch (ex)
            {
                case ValidationException validationException:
                    _logger.LogWarning(ex, "Validation error occurred");
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.StatusCode = response.StatusCode;
                    errorResponse.Message = "Validation failed";
                    errorResponse.Errors = validationException.Errors
                        .Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
                        .ToList();
                    break;

                case NotFoundException notFoundException:
                    _logger.LogWarning(ex, "Resource not found: {Message}", ex.Message);
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse.StatusCode = response.StatusCode;
                    errorResponse.Message = notFoundException.Message;
                    break;

                case UnauthorizedAccessException:
                    _logger.LogWarning(ex, "Unauthorized access attempt");
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    errorResponse.StatusCode = response.StatusCode;
                    errorResponse.Message = "Access denied";
                    break;

                case InvalidOperationException invalidOpException:
                    _logger.LogWarning(ex, "Invalid operation: {Message}", ex.Message);
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.StatusCode = response.StatusCode;
                    errorResponse.Message = invalidOpException.Message;
                    break;

                case ArgumentException argumentException:
                    _logger.LogWarning(ex, "Invalid argument: {Message}", ex.Message);
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.StatusCode = response.StatusCode;
                    errorResponse.Message = argumentException.Message;
                    break;

                default:
                    // This is a critical unhandled exception
                    _logger.LogError(ex, "Unhandled exception occurred: {Message}", ex.Message);
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.StatusCode = response.StatusCode;
                    errorResponse.Message = _env.IsDevelopment()
                        ? ex.Message
                        : "An error occurred while processing your request";

                    if (_env.IsDevelopment())
                    {
                        errorResponse.Errors = new List<string>
                        {
                            ex.StackTrace ?? string.Empty
                        };
                    }
                    break;
            }

            // Serialize and write the response
            var jsonResponse = System.Text.Json.JsonSerializer.Serialize(errorResponse, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            });

            await response.WriteAsync(jsonResponse);
        }
    }
}
