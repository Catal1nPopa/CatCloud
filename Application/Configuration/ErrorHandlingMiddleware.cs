using Application.Configuration.ExceptionConfig.Exceptions;
using Microsoft.AspNetCore.Http;
using Npgsql;
using System.Net;
using System.Text.Json;

namespace Application.Configuration.ExceptionConfig
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _requestDelegate;

        public ErrorHandlingMiddleware(RequestDelegate requestDelegate)
        {
            _requestDelegate = requestDelegate;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _requestDelegate(context);
            }
            catch (PostgresException ex)
            {
                await HandleExceptionAsync(context, new Exception("DB-ERROR"));
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }

        }
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode statusCode;
            string message = exception.Message;

            var typeException = exception.GetType();

            switch (typeException.Name)
            {
                case nameof(BadRequestException):
                    statusCode = HttpStatusCode.BadRequest;
                    break;
                case nameof(Exceptions.NotImplementedException):
                    statusCode = HttpStatusCode.NotImplemented;
                    break;
                case nameof(NotFoundException):
                    statusCode = HttpStatusCode.NotFound;
                    break;
                case nameof(Exceptions.UnauthorizedAccessException):
                    statusCode = HttpStatusCode.Unauthorized;
                    break;
                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;
            //Log.Information("Code: (@statusCode) => Message: {@message}", statusCode, message);
            return context.Response.WriteAsync(JsonSerializer.Serialize(new { message, statusCode }));
        }
    }
}
