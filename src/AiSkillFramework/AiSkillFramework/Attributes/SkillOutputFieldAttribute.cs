namespace AiSkillFramework.Attributes;

/// <summary>
/// 描述 Skill 函数的一个输出字段，用于文档生成。
/// 可在同一方法上多次使用，每次对应一个输出字段。
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class SkillOutputFieldAttribute : Attribute
{
    /// <summary>
    /// 字段名称。
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 字段类型描述（如 "str"、"float"、"int"）。
    /// </summary>
    public string Type { get; set; } = "str";

    /// <summary>
    /// 是否默认返回（Y/N）。
    /// </summary>
    public bool DefaultDisplay { get; set; } = true;

    /// <summary>
    /// 字段说明。
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <param name="name">字段名称。</param>
    public SkillOutputFieldAttribute(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("输出字段名称不能为空。", nameof(name));
        Name = name;
    }
}
