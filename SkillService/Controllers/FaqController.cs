using Microsoft.AspNetCore.Mvc;
using SkillService.Models;
using SkillService.Services;

namespace SkillService.Controllers;

[ApiController]
[Route("api/skill")]
public class FaqController : ControllerBase
{
    private readonly IFaqService _faqService;

    public FaqController(IFaqService faqService) => _faqService = faqService;

    /// <summary>
    /// POST /api/skill/faq-match
    /// Matches the incoming question against the FAQ knowledge base.
    /// Returns a structured response with matched, answer, category, faqId.
    /// </summary>
    [HttpPost("faq-match")]
    public IActionResult FaqMatch([FromBody] FaqRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Question))
            return BadRequest(new { error = "Question is required." });

        // Check skill-level permission
        var client = HttpContext.Items["ClientInfo"] as ClientInfo;
        if (client is not null && !client.AllowedSkills.Contains("faq-match"))
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                new { error = "Client is not authorized for skill 'faq-match'." });
        }

        var response = _faqService.Match(request.Question);
        return Ok(response);
    }
}
