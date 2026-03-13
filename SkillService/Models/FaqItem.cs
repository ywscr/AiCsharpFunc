namespace SkillService.Models;

/// <summary>
/// Represents a single FAQ rule stored in the knowledge base.
/// </summary>
public class FaqItem
{
    public string Id { get; set; } = "";
    public string Category { get; set; } = "";

    /// <summary>
    /// Pattern syntax:
    ///   Groups separated by ';' represent AND conditions.
    ///   Within each group, terms separated by '|' represent OR conditions.
    /// Example: "退款|退货;多久|几天|期限" means (退款 OR 退货) AND (多久 OR 几天 OR 期限)
    /// </summary>
    public string Pattern { get; set; } = "";

    public string Answer { get; set; } = "";
    public int Priority { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}
