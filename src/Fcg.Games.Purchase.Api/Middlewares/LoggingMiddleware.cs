using System.Text;

namespace Fcg.Games.Purchase.Api.Middlewares;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var request = context.Request;
        context.Request.EnableBuffering();

        Console.WriteLine($"[Request] {request.Method} {request.Path}");

        if (request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            Console.WriteLine("[Authorization]");
            Console.WriteLine(authHeader);
        }

        string bodyAsText = await GetBodyFromRequestAsync(request);
        if (!string.IsNullOrWhiteSpace(bodyAsText))
        {
            Console.WriteLine("[Body]");
            Console.WriteLine(bodyAsText);
        }

        await _next(context);
    }

    private static async Task<string> GetBodyFromRequestAsync(HttpRequest request)
    {
        if (request.ContentLength > 0 && request.Body.CanRead)
        {
            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            var bodyAsText = await reader.ReadToEndAsync();
            request.Body.Position = 0;
            return bodyAsText;
        }

        return string.Empty;
    }
}
