using SkillService.Models;
using SkillService.Services;
using System.Text;

namespace SkillService.Middleware;

/// <summary>
/// ASP.NET Core middleware that enforces service-to-service authentication
/// on all routes under /api/skill/*.
///
/// Required headers:
///   X-Client-Id  – registered client identifier
///   X-Api-Key    – static API key for the client
///   X-Timestamp  – Unix epoch seconds (UTC)
///   X-Nonce      – unique UUID per request (anti-replay)
///   X-Signature  – HMAC-SHA256 hex of "{timestamp}\n{nonce}\n{body}"
/// </summary>
public class SkillAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SkillAuthMiddleware> _logger;

    public SkillAuthMiddleware(RequestDelegate next, ILogger<SkillAuthMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IAuthService authService)
    {
        // Only guard /api/skill/* routes
        if (!context.Request.Path.StartsWithSegments("/api/skill", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        var reqHeaders = context.Request.Headers;
        var clientId   = reqHeaders["X-Client-Id"].FirstOrDefault()  ?? "";
        var apiKey     = reqHeaders["X-Api-Key"].FirstOrDefault()    ?? "";
        var timestamp  = reqHeaders["X-Timestamp"].FirstOrDefault()  ?? "";
        var nonce      = reqHeaders["X-Nonce"].FirstOrDefault()      ?? "";
        var signature  = reqHeaders["X-Signature"].FirstOrDefault()  ?? "";

        if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(apiKey) ||
            string.IsNullOrEmpty(timestamp) || string.IsNullOrEmpty(nonce) ||
            string.IsNullOrEmpty(signature))
        {
            _logger.LogWarning("Rejected request: missing authentication headers.");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = "Missing authentication headers." });
            return;
        }

        // Buffer the request body so it can be read both here and downstream
        context.Request.EnableBuffering();
        string requestBody;
        using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
        {
            requestBody = await reader.ReadToEndAsync();
        }
        context.Request.Body.Position = 0;

        var headers = new AuthHeaders(clientId, apiKey, timestamp, nonce, signature);
        var result  = authService.Authenticate(headers, requestBody);

        if (!result.Success)
        {
            _logger.LogWarning("Skill auth rejected for '{ClientId}': {Reason}", clientId, result.FailureReason);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = result.FailureReason });
            return;
        }

        // Make authenticated client info available to controllers
        context.Items["ClientInfo"] = result.Client;
        await _next(context);
    }
}
