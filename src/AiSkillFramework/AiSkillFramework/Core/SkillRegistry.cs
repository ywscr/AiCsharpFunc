using System.Collections.Concurrent;
using System.Reflection;
using AiSkillFramework.Attributes;
using AiSkillFramework.Models;

namespace AiSkillFramework.Core;

/// <summary>
/// Skill 注册表：负责发现、注册并提供 <see cref="SkillInfo"/> 元数据。
/// </summary>
public static class SkillRegistry
{
    private static readonly ConcurrentDictionary<Type, SkillInfo> _cache = new();

    /// <summary>
    /// 从程序集中扫描所有带有 <see cref="SkillAttribute"/> 的类，并返回对应的
    /// <see cref="SkillInfo"/> 列表。
    /// </summary>
    /// <param name="assembly">要扫描的程序集。若为 null，则扫描调用方程序集。</param>
    public static IReadOnlyList<SkillInfo> DiscoverSkills(Assembly? assembly = null)
    {
        assembly ??= Assembly.GetCallingAssembly();
        return assembly.GetTypes()
            .Where(t => t.GetCustomAttribute<SkillAttribute>() is not null)
            .Select(BuildSkillInfo)
            .ToList();
    }

    /// <summary>
    /// 根据 CLR 类型构建（或从缓存返回）对应的 <see cref="SkillInfo"/>。
    /// </summary>
    /// <param name="skillType">带有 <see cref="SkillAttribute"/> 注解的类型。</param>
    /// <exception cref="InvalidOperationException">类型未标注 <see cref="SkillAttribute"/> 时抛出。</exception>
    public static SkillInfo BuildSkillInfo(Type skillType)
    {
        return _cache.GetOrAdd(skillType, BuildSkillInfoCore);
    }

    private static SkillInfo BuildSkillInfoCore(Type skillType)
    {
        var skillAttr = skillType.GetCustomAttribute<SkillAttribute>()
            ?? throw new InvalidOperationException(
                $"类型 '{skillType.FullName}' 未标注 [Skill] 特性，无法构建 SkillInfo。");

        var functions = skillType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(m => BuildFunctionInfo(m))
            .Where(f => f is not null)
            .Cast<SkillFunctionInfo>()
            .ToList();

        return new SkillInfo
        {
            Name = skillAttr.Name,
            Description = skillAttr.Description,
            Version = skillAttr.Version,
            Author = skillAttr.Author,
            Functions = functions,
            SkillType = skillType
        };
    }

    private static SkillFunctionInfo? BuildFunctionInfo(MethodInfo method)
    {
        var funcAttr = method.GetCustomAttribute<SkillFunctionAttribute>();
        if (funcAttr is null) return null;

        var parameters = method.GetCustomAttributes<SkillParameterAttribute>()
            .Select(p => new SkillParameterInfo
            {
                Name = p.Name,
                Type = p.Type,
                Required = p.Required,
                Description = p.Description
            })
            .ToList();

        var outputFields = method.GetCustomAttributes<SkillOutputFieldAttribute>()
            .Select(o => new SkillOutputFieldInfo
            {
                Name = o.Name,
                Type = o.Type,
                DefaultDisplay = o.DefaultDisplay,
                Description = o.Description
            })
            .ToList();

        return new SkillFunctionInfo
        {
            MethodName = method.Name,
            ApiName = funcAttr.ApiName ?? method.Name,
            Description = funcAttr.Description,
            Category = funcAttr.Category,
            Permission = funcAttr.Permission,
            Parameters = parameters,
            OutputFields = outputFields,
            Method = method
        };
    }
}
