using System.Reflection;

namespace AiSkillFramework.Models;

/// <summary>
/// 描述一个 Skill 函数（接口）的元数据。
/// </summary>
public class SkillFunctionInfo
{
    /// <summary>方法名称（C# 方法名）。</summary>
    public string MethodName { get; init; } = string.Empty;

    /// <summary>API 接口名称（如 "stock_basic"）。</summary>
    public string ApiName { get; init; } = string.Empty;

    /// <summary>接口描述。</summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>所属分类（如 "股票数据/基础数据"）。</summary>
    public string Category { get; init; } = string.Empty;

    /// <summary>权限说明。</summary>
    public string Permission { get; init; } = string.Empty;

    /// <summary>输入参数列表。</summary>
    public IReadOnlyList<SkillParameterInfo> Parameters { get; init; } = [];

    /// <summary>输出字段列表。</summary>
    public IReadOnlyList<SkillOutputFieldInfo> OutputFields { get; init; } = [];

    /// <summary>对应的 CLR 方法信息。</summary>
    public MethodInfo Method { get; init; } = null!;
}
