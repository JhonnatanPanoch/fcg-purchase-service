using Fcg.Games.Purchase.Domain.Exceptions;
using Fcg.Games.Purchase.Domain.Exceptions.Responses;
using System.Net;
using System.Text.Json;

namespace Fcg.Games.Purchase.Api.Middlewares;
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ocorreu uma exceção: {ex.Message}");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = GetResponseStatusCode(exception);

        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var isDevelopment = env == Environments.Development;

        var errors = new Dictionary<string, string[]>();

        if (isDevelopment && exception.InnerException != null)
            errors.Add("InnerException", [exception.InnerException.ToString()]);

        var jsonResponse = JsonSerializer.Serialize(
            new ErrorResponse(
                context.Response.StatusCode,
                exception.Message,
                errors));

        return context.Response.WriteAsync(jsonResponse);
    }



    private static int GetResponseStatusCode(Exception exception) => exception switch
    {
        NotFoundException => (int)HttpStatusCode.NotFound,
        BussinessException => (int)HttpStatusCode.BadRequest,
        AuthenticationException => (int)HttpStatusCode.Unauthorized,
        ConflictException => (int)HttpStatusCode.Conflict,
        _ => (int)HttpStatusCode.InternalServerError,
    };
}