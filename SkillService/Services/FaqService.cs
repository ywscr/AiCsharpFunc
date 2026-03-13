using SkillService.Data;
using SkillService.Models;

namespace SkillService.Services;

/// <summary>
/// FAQ matching service.
/// Pattern syntax: groups separated by ';' are AND conditions;
/// within each group, terms separated by '|' are OR conditions.
/// Example: "退款|退货;多久|几天" = (退款 OR 退货) AND (多久 OR 几天)
/// </summary>
public class FaqService : IFaqService
{
    private readonly FaqStore _store;

    public FaqService(FaqStore store) => _store = store;

    public FaqResponse Match(string question)
    {
        var normalized = question.Trim().ToLowerInvariant();

        var hit = _store.Items
            .Where(item => item.IsActive && MatchesPattern(normalized, item.Pattern))
            .OrderByDescending(item => item.Priority)
            .FirstOrDefault();

        if (hit is null)
            return new FaqResponse { Matched = false };

        return new FaqResponse
        {
            Matched   = true,
            Answer    = hit.Answer,
            Category  = hit.Category,
            FaqId     = hit.Id,
        };
    }

    private static bool MatchesPattern(string text, string pattern)
    {
        // Each ';'-delimited group must match (AND)
        var andGroups = pattern.Split(';', StringSplitOptions.RemoveEmptyEntries);
        return andGroups.All(group =>
        {
            // At least one '|'-delimited term in the group must be present (OR)
            var orTerms = group.Split('|', StringSplitOptions.RemoveEmptyEntries);
            return orTerms.Any(term => text.Contains(term.Trim(), StringComparison.OrdinalIgnoreCase));
        });
    }
}
