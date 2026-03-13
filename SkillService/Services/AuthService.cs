using SkillService.Data;
using SkillService.Models;
using System.Security.Cryptography;
using System.Text;

namespace SkillService.Services;

/// <summary>
/// Validates service-to-service requests using API key + HMAC-SHA256 signatures.
///
/// Signature input (UTF-8): "{timestamp}\n{nonce}\n{requestBody}"
/// Signature algorithm    : HMAC-SHA256(key=HmacSecret, msg=above)
/// Signature encoding     : lowercase hex
/// </summary>
public class AuthService : IAuthService
{
    private readonly IReadOnlyDictionary<string, ClientInfo> _clients;
    private readonly NonceStore _nonceStore;
    private readonly ILogger<AuthService> _logger;

    private static readonly TimeSpan MaxTimestampAge = TimeSpan.FromMinutes(5);

    public AuthService(
        IConfiguration configuration,
        NonceStore nonceStore,
        ILogger<AuthService> logger)
    {
        _nonceStore = nonceStore;
        _logger     = logger;
        _clients    = configuration
            .GetSection("Clients")
            .Get<List<ClientInfo>>()
            ?.ToDictionary(c => c.ClientId, c => c)
            ?? new Dictionary<string, ClientInfo>();
    }

    public AuthResult Authenticate(AuthHeaders headers, string requestBody)
    {
        // 1. Look up client
        if (!_clients.TryGetValue(headers.ClientId, out var client))
        {
            _logger.LogWarning("Auth failed: unknown ClientId '{ClientId}'.", headers.ClientId);
            return Fail("Unknown client.");
        }

        // 2. Validate API key (constant-time to avoid timing attacks)
        if (!ConstantTimeEquals(client.ApiKey, headers.ApiKey))
        {
            _logger.LogWarning("Auth failed: invalid ApiKey for client '{ClientId}'.", headers.ClientId);
            return Fail("Invalid API key.");
        }

        // 3. Validate timestamp (within ±5 minutes)
        if (!long.TryParse(headers.Timestamp, out var tsSeconds))
            return Fail("Invalid timestamp format.");

        var requestTime = DateTimeOffset.FromUnixTimeSeconds(tsSeconds);
        var age = DateTimeOffset.UtcNow - requestTime;
        if (age < -MaxTimestampAge || age > MaxTimestampAge)
        {
            _logger.LogWarning(
                "Auth failed: timestamp out of range for client '{ClientId}'. Age={Age}.",
                headers.ClientId, age);
            return Fail("Timestamp out of valid range.");
        }

        // 4. Validate nonce (anti-replay)
        var nonceKey = $"{headers.ClientId}:{headers.Nonce}";
        if (!_nonceStore.TryAddNonce(nonceKey))
        {
            _logger.LogWarning(
                "Auth failed: nonce replay for client '{ClientId}'. Nonce='{Nonce}'.",
                headers.ClientId, headers.Nonce);
            return Fail("Nonce already used.");
        }

        // 5. Validate HMAC-SHA256 signature
        var expected = ComputeHmac(client.HmacSecret, headers.Timestamp, headers.Nonce, requestBody);
        if (!ConstantTimeEquals(expected, headers.Signature))
        {
            _logger.LogWarning("Auth failed: signature mismatch for client '{ClientId}'.", headers.ClientId);
            return Fail("Invalid signature.");
        }

        return new AuthResult { Success = true, Client = client };
    }

    private static string ComputeHmac(string secret, string timestamp, string nonce, string body)
    {
        var message  = $"{timestamp}\n{nonce}\n{body}";
        var keyBytes = Encoding.UTF8.GetBytes(secret);
        var msgBytes = Encoding.UTF8.GetBytes(message);
        using var hmac = new HMACSHA256(keyBytes);
        return Convert.ToHexString(hmac.ComputeHash(msgBytes)).ToLowerInvariant();
    }

    /// <summary>
    /// Constant-time string comparison to mitigate timing attacks.
    /// </summary>
    private static bool ConstantTimeEquals(string a, string b)
    {
        var bytesA = Encoding.UTF8.GetBytes(a);
        var bytesB = Encoding.UTF8.GetBytes(b);
        return CryptographicOperations.FixedTimeEquals(bytesA, bytesB);
    }

    private static AuthResult Fail(string reason) =>
        new() { Success = false, FailureReason = reason };
}
