namespace AiSkillFramework.Attributes;

/// <summary>
/// 标记一个方法为 Skill 的可调用函数（接口），用于向 AI 代理暴露具体操作。
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class SkillFunctionAttribute : Attribute
{
    /// <summary>
    /// 接口名称（通常与底层 API 的方法名一致，如 "stock_basic"）。
    /// 若不指定，则使用被标注方法的名称。
    /// </summary>
    public string? ApiName { get; set; }

    /// <summary>
    /// 接口描述，向 AI 说明该函数的用途。
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 该接口所属的分类（如 "股票数据/基础数据"）。
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// 调用该接口所需的最低积分或权限说明。
    /// </summary>
    public string Permission { get; set; } = string.Empty;
}
