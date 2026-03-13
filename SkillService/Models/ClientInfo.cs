namespace SkillService.Models;

/// <summary>
/// Represents a registered API client (e.g., the AI Gateway) with its credentials and skill permissions.
/// </summary>
public class ClientInfo
{
    public string ClientId { get; set; } = "";
    public string ApiKey { get; set; } = "";
    public string HmacSecret { get; set; } = "";

    /// <summary>
    /// Explicit list of skills this client is allowed to call (e.g., ["faq-match"]).
    /// </summary>
    public string[] AllowedSkills { get; set; } = Array.Empty<string>();
}
