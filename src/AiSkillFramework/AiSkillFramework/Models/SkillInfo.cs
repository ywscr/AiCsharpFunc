namespace AiSkillFramework.Models;

/// <summary>
/// 描述一个 Skill 的元数据。
/// </summary>
public class SkillInfo
{
    /// <summary>技能名称（英文标识符）。</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>技能描述。</summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>技能版本号。</summary>
    public string Version { get; init; } = "1.0.0";

    /// <summary>技能作者。</summary>
    public string Author { get; init; } = string.Empty;

    /// <summary>该技能包含的所有函数信息。</summary>
    public IReadOnlyList<SkillFunctionInfo> Functions { get; init; } = [];

    /// <summary>实现该技能的 CLR 类型。</summary>
    public Type SkillType { get; init; } = typeof(object);
}
