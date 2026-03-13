using SkillService.Models;
using System.Text.Json;

namespace SkillService.Data;

/// <summary>
/// In-memory FAQ store, loaded from seed-data/seed-faqs.json at startup.
/// Extend this class to load from a database or external config service.
/// </summary>
public class FaqStore
{
    public IReadOnlyList<FaqItem> Items { get; }

    public FaqStore(IWebHostEnvironment env, ILogger<FaqStore> logger)
    {
        var seedFile = Path.Combine(env.ContentRootPath, "seed-data", "seed-faqs.json");
        if (File.Exists(seedFile))
        {
            var json = File.ReadAllText(seedFile);
            var items = JsonSerializer.Deserialize<List<FaqItem>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Items = items ?? new List<FaqItem>();
            logger.LogInformation("Loaded {Count} FAQ items from {File}.", Items.Count, seedFile);
        }
        else
        {
            Items = DefaultFaqs();
            logger.LogInformation("seed-faqs.json not found; loaded {Count} built-in FAQ items.", Items.Count);
        }
    }

    private static List<FaqItem> DefaultFaqs() =>
    [
        new() { Id = "faq-001", Category = "refund",   Priority = 10, Pattern = "退款|退货;期限|多久|几天|时间",    Answer = "退款申请需在签收后 7 天内提交。" },
        new() { Id = "faq-002", Category = "invoice",  Priority = 10, Pattern = "发票;邮箱|发送|哪里",             Answer = "发票统一发送到 finance@example.com。" },
        new() { Id = "faq-003", Category = "support",  Priority = 10, Pattern = "客服|人工;时间|几点|上班",        Answer = "人工客服服务时间为工作日 09:00-18:00。" },
        new() { Id = "faq-004", Category = "refund",   Priority = 8,  Pattern = "退款;流程|步骤|怎么|如何",        Answer = "退款流程：登录账户 → 订单详情 → 申请退款 → 填写原因 → 提交。审核通过后 3-5 个工作日退回。" },
        new() { Id = "faq-005", Category = "shipping", Priority = 10, Pattern = "快递|物流|配送;多久|几天|时间",   Answer = "标准配送 3-5 个工作日，加急配送 1-2 个工作日。" },
    ];
}
