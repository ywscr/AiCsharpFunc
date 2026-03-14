namespace AiSkillFramework.Models;

/// <summary>
/// 描述一个 Skill 函数的输出字段元数据。
/// </summary>
public class SkillOutputFieldInfo
{
    /// <summary>字段名称。</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>字段类型描述（如 "str"、"float"、"int"）。</summary>
    public string Type { get; init; } = "str";

    /// <summary>是否默认返回。</summary>
    public bool DefaultDisplay { get; init; } = true;

    /// <summary>字段说明。</summary>
    public string Description { get; init; } = string.Empty;
}
