using System.Text;
using AiSkillFramework.Models;

namespace AiSkillFramework.Core;

/// <summary>
/// 根据 <see cref="SkillInfo"/> 生成 SKILL.md 格式的文档。
/// 生成的文档与 waditu-tushare/skills 项目的文档风格保持一致。
/// </summary>
public static class SkillDocumentGenerator
{
    /// <summary>
    /// 生成完整的 SKILL.md 内容字符串。
    /// </summary>
    /// <param name="skill">技能元数据。</param>
    /// <returns>Markdown 格式的 SKILL.md 内容。</returns>
    public static string Generate(SkillInfo skill)
    {
        var sb = new StringBuilder();

        // YAML frontmatter
        sb.AppendLine("---");
        sb.AppendLine($"name: {skill.Name}");
        sb.AppendLine($"description: {skill.Description}");
        if (!string.IsNullOrWhiteSpace(skill.Version))
            sb.AppendLine($"version: {skill.Version}");
        if (!string.IsNullOrWhiteSpace(skill.Author))
            sb.AppendLine($"author: {skill.Author}");
        sb.AppendLine("---");
        sb.AppendLine();

        // 标题
        sb.AppendLine($"# {skill.Name}");
        sb.AppendLine();

        // 概述
        sb.AppendLine("## 概述");
        sb.AppendLine();
        sb.AppendLine(skill.Description);
        sb.AppendLine();

        // 数据接口列表
        sb.AppendLine("## 数据接口列表");
        sb.AppendLine();

        if (skill.Functions.Count > 0)
        {
            sb.AppendLine("| 接口名 | 分类 | 描述 |");
            sb.AppendLine("| :----- | :--- | :--- |");

            foreach (var func in skill.Functions.OrderBy(f => f.Category).ThenBy(f => f.ApiName))
            {
                var category = string.IsNullOrWhiteSpace(func.Category) ? "-" : func.Category;
                var desc = string.IsNullOrWhiteSpace(func.Description) ? "-" : func.Description;
                sb.AppendLine($"| {func.ApiName} | {category} | {desc} |");
            }

            sb.AppendLine();
        }

        // 接口详情
        sb.AppendLine("## 接口详情");
        sb.AppendLine();

        foreach (var func in skill.Functions.OrderBy(f => f.Category).ThenBy(f => f.ApiName))
        {
            AppendFunctionDoc(sb, func);
        }

        return sb.ToString();
    }

    /// <summary>
    /// 将 SKILL.md 内容写入到指定文件。
    /// </summary>
    /// <param name="skill">技能元数据。</param>
    /// <param name="outputPath">输出文件路径（如 "SKILL.md"）。</param>
    public static void GenerateToFile(SkillInfo skill, string outputPath)
    {
        var content = Generate(skill);
        File.WriteAllText(outputPath, content, Encoding.UTF8);
    }

    private static void AppendFunctionDoc(StringBuilder sb, SkillFunctionInfo func)
    {
        sb.AppendLine($"### {func.ApiName}");
        sb.AppendLine();

        if (!string.IsNullOrWhiteSpace(func.Description))
        {
            sb.AppendLine($"接口：{func.ApiName}");
            sb.AppendLine($"描述：{func.Description}");
            if (!string.IsNullOrWhiteSpace(func.Permission))
                sb.AppendLine($"权限：{func.Permission}");
            sb.AppendLine();
        }

        if (func.Parameters.Count > 0)
        {
            sb.AppendLine("**输入参数**");
            sb.AppendLine();
            sb.AppendLine("| 名称 | 类型 | 必选 | 描述 |");
            sb.AppendLine("| :--- | :--- | :--: | :--- |");

            foreach (var p in func.Parameters)
            {
                var required = p.Required ? "Y" : "N";
                sb.AppendLine($"| {p.Name} | {p.Type} | {required} | {p.Description} |");
            }

            sb.AppendLine();
        }

        if (func.OutputFields.Count > 0)
        {
            sb.AppendLine("**输出参数**");
            sb.AppendLine();
            sb.AppendLine("| 名称 | 类型 | 默认显示 | 描述 |");
            sb.AppendLine("| :--- | :--- | :------: | :--- |");

            foreach (var o in func.OutputFields)
            {
                var display = o.DefaultDisplay ? "Y" : "N";
                sb.AppendLine($"| {o.Name} | {o.Type} | {display} | {o.Description} |");
            }

            sb.AppendLine();
        }
    }
}
