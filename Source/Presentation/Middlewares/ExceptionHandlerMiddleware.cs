using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Presentation.Middlewares;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception e)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = e switch
            {
                InvalidEmailFormatException => (int) HttpStatusCode.BadRequest,
                InvalidPasswordException => (int) HttpStatusCode.BadRequest,
                InvalidPasswordFormatException => (int) HttpStatusCode.BadRequest,
                UnauthorizedAccessException => (int) HttpStatusCode.Unauthorized,
                UserAlreadyRegisteredException => (int) HttpStatusCode.Conflict,
                UserNotFoundException => (int) HttpStatusCode.NotFound,
                _ => (int) HttpStatusCode.InternalServerError
            };
            await response.WriteAsync(JsonSerializer.Serialize(new {e.Message}));
        }
    }
}