namespace SkillService.Models;

/// <summary>
/// Structured response returned by the FAQ skill.
/// </summary>
public class FaqResponse
{
    public bool Matched { get; set; }
    public string Answer { get; set; } = "";
    public string Category { get; set; } = "";
    public string FaqId { get; set; } = "";
}
