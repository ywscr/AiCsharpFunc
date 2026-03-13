namespace SkillService.Data;

/// <summary>
/// Thread-safe in-memory nonce store to prevent replay attacks.
/// A nonce is accepted once within a rolling time window.
/// </summary>
public class NonceStore
{
    private readonly Dictionary<string, DateTimeOffset> _seen = new();
    private readonly object _lock = new();
    private readonly TimeSpan _retentionWindow = TimeSpan.FromMinutes(10);

    /// <summary>
    /// Attempts to register a nonce. Returns false if the nonce was already seen.
    /// </summary>
    public bool TryAddNonce(string nonce)
    {
        lock (_lock)
        {
            var now = DateTimeOffset.UtcNow;
            PurgeExpired(now);

            if (_seen.ContainsKey(nonce))
                return false;

            _seen[nonce] = now;
            return true;
        }
    }

    private void PurgeExpired(DateTimeOffset now)
    {
        var cutoff = now - _retentionWindow;
        foreach (var key in _seen.Where(kv => kv.Value < cutoff).Select(kv => kv.Key).ToList())
            _seen.Remove(key);
    }
}
