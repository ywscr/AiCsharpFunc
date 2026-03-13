using SkillService.Models;

namespace SkillService.Services;

public interface IFaqService
{
    FaqResponse Match(string question);
}
