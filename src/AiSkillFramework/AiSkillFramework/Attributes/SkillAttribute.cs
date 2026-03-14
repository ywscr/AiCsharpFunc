namespace AiSkillFramework.Attributes;

/// <summary>
/// 标记一个类为 AI Skill，用于声明技能的基本信息。
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class SkillAttribute : Attribute
{
    /// <summary>
    /// 技能名称（英文标识符，如 "tushare"）。
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 技能描述（中文或英文均可）。
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 技能版本号。
    /// </summary>
    public string Version { get; set; } = "1.0.0";

    /// <summary>
    /// 技能作者。
    /// </summary>
    public string Author { get; set; } = string.Empty;

    /// <param name="name">技能名称（英文标识符）。</param>
    public SkillAttribute(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("技能名称不能为空。", nameof(name));
        Name = name;
    }
}
