namespace AiSkillFramework.Attributes;

/// <summary>
/// 描述 Skill 函数的一个输入参数，用于文档生成和参数校验。
/// 可在同一方法上多次使用，每次对应一个参数。
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class SkillParameterAttribute : Attribute
{
    /// <summary>
    /// 参数名称。
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 参数类型描述（如 "str"、"int"、"date(YYYYMMDD)"）。
    /// </summary>
    public string Type { get; set; } = "str";

    /// <summary>
    /// 是否为必填参数。
    /// </summary>
    public bool Required { get; set; } = false;

    /// <summary>
    /// 参数说明。
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <param name="name">参数名称。</param>
    public SkillParameterAttribute(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("参数名称不能为空。", nameof(name));
        Name = name;
    }
}
