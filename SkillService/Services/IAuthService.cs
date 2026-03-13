using SkillService.Models;

namespace SkillService.Services;

public interface IAuthService
{
    AuthResult Authenticate(AuthHeaders headers, string requestBody);
}

public class AuthResult
{
    public bool Success { get; init; }
    public string? FailureReason { get; init; }
    public ClientInfo? Client { get; init; }
}

/// <summary>
/// Authentication headers extracted from an incoming HTTP request.
/// </summary>
public record AuthHeaders(
    string ClientId,
    string ApiKey,
    string Timestamp,
    string Nonce,
    string Signature);
