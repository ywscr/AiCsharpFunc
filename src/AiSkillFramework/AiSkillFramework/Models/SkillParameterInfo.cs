namespace AiSkillFramework.Models;

/// <summary>
/// 描述一个 Skill 函数的输入参数元数据。
/// </summary>
public class SkillParameterInfo
{
    /// <summary>参数名称。</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>参数类型描述（如 "str"、"int"、"date(YYYYMMDD)"）。</summary>
    public string Type { get; init; } = "str";

    /// <summary>是否必填。</summary>
    public bool Required { get; init; } = false;

    /// <summary>参数说明。</summary>
    public string Description { get; init; } = string.Empty;
}
