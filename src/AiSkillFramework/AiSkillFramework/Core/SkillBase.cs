using AiSkillFramework.Attributes;
using AiSkillFramework.Models;

namespace AiSkillFramework.Core;

/// <summary>
/// AI Skill 的抽象基类。所有 Skill 实现类应继承此类，并用 <see cref="SkillAttribute"/> 标注。
/// 基类提供技能元数据访问及简单的参数验证辅助方法。
/// </summary>
public abstract class SkillBase
{
    private SkillInfo? _skillInfo;

    /// <summary>
    /// 获取当前 Skill 的元数据。元数据通过反射从 <see cref="SkillAttribute"/> 及
    /// <see cref="SkillFunctionAttribute"/> 中读取，结果会被缓存。
    /// </summary>
    public SkillInfo GetSkillInfo()
    {
        return _skillInfo ??= SkillRegistry.BuildSkillInfo(GetType());
    }

    /// <summary>
    /// 验证必填参数不为 null 或空字符串。
    /// </summary>
    /// <param name="value">参数值。</param>
    /// <param name="paramName">参数名称（用于错误提示）。</param>
    /// <exception cref="ArgumentException">当必填参数为空时抛出。</exception>
    protected static void RequireParameter(string? value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"参数 '{paramName}' 是必填项，不能为空。", paramName);
    }

    /// <summary>
    /// 验证日期字符串格式是否为 yyyyMMdd。
    /// </summary>
    /// <param name="date">日期字符串。</param>
    /// <param name="paramName">参数名称（用于错误提示）。</param>
    /// <exception cref="ArgumentException">当日期格式不正确时抛出。</exception>
    protected static void RequireDateFormat(string? date, string paramName)
    {
        if (date is null) return;
        if (!DateTime.TryParseExact(date, "yyyyMMdd",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out _))
            throw new ArgumentException(
                $"参数 '{paramName}' 的日期格式应为 yyyyMMdd（如 20241231），当前值：'{date}'。",
                paramName);
    }
}
