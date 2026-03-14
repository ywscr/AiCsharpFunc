using AiSkillFramework.Core;
using TushareSkillSample.Skills;

// -----------------------------------------------------------------------
// 演示如何使用 AiSkillFramework 快速创建和使用 AI Skill
// -----------------------------------------------------------------------

Console.WriteLine("=== AiSkillFramework 示例 ===");
Console.WriteLine();

// 1. 创建 Skill 实例
var tushare = new TushareSkill();

// 2. 获取 Skill 元数据
var skillInfo = tushare.GetSkillInfo();

Console.WriteLine($"技能名称  : {skillInfo.Name}");
Console.WriteLine($"技能版本  : {skillInfo.Version}");
Console.WriteLine($"接口数量  : {skillInfo.Functions.Count}");
Console.WriteLine();

// 3. 列出所有接口
Console.WriteLine("--- 接口列表 ---");
foreach (var func in skillInfo.Functions)
{
    Console.WriteLine($"  [{func.Category}] {func.ApiName} — {func.Description}");
    Console.WriteLine($"    入参数量: {func.Parameters.Count}  出参数量: {func.OutputFields.Count}");
}
Console.WriteLine();

// 4. 调用接口示例
Console.WriteLine("--- 调用示例 ---");

var stockList = await tushare.GetStockBasicAsync(listStatus: "L");
Console.WriteLine($"stock_basic 结果: {stockList}");

var daily = await tushare.GetDailyAsync("000001.SZ", startDate: "20241201", endDate: "20241231");
Console.WriteLine($"daily 结果      : {daily}");

var gdp = await tushare.GetCnGdpAsync(startQ: "2020Q1");
Console.WriteLine($"cn_gdp 结果     : {gdp}");
Console.WriteLine();

// 5. 生成 SKILL.md 文档
Console.WriteLine("--- 生成 SKILL.md ---");
var skillMd = SkillDocumentGenerator.Generate(skillInfo);
var outputPath = Path.Combine(AppContext.BaseDirectory, "SKILL.md");
SkillDocumentGenerator.GenerateToFile(skillInfo, outputPath);
Console.WriteLine($"SKILL.md 已生成到: {outputPath}");
Console.WriteLine();

// 6. 预览文档前 30 行
Console.WriteLine("--- SKILL.md 预览（前 30 行）---");
foreach (var line in skillMd.Split(new[] { '\r', '\n' }, StringSplitOptions.None).Take(30))
    Console.WriteLine(line);

Console.WriteLine();
Console.WriteLine("=== 完成 ===");
