# AiCsharpFunc
AI + C#

## AiSkillFramework

**AiSkillFramework** 是一个 C# AI Skill 封装框架，灵感来源于 [waditu-tushare/skills](https://github.com/waditu-tushare/skills) 项目，旨在高度封装接口，便于快速创建新的类似 Skill。

### 特性

- 🏷️ **属性驱动** — 用 `[Skill]`、`[SkillFunction]`、`[SkillParameter]`、`[SkillOutputField]` 描述技能与接口
- 🔍 **自动发现** — `SkillRegistry` 通过反射自动扫描并构建元数据
- 📄 **文档生成** — `SkillDocumentGenerator` 自动生成与 tushare/skills 风格一致的 `SKILL.md`
- 🧩 **易于扩展** — 继承 `SkillBase`，几行代码即可接入新接口

### 快速上手

```csharp
// 1. 定义 Skill
[Skill("my_skill", Description = "我的 AI 技能")]
public class MySkill : SkillBase
{
    [SkillFunction(ApiName = "hello", Description = "打个招呼", Category = "示例")]
    [SkillParameter("name", Type = "str", Required = true, Description = "姓名")]
    [SkillOutputField("message", Type = "str", Description = "招呼消息")]
    public Task<string> HelloAsync(string name)
    {
        RequireParameter(name, nameof(name));
        return Task.FromResult($"Hello, {name}!");
    }
}

// 2. 获取元数据
var skill = new MySkill();
var info = skill.GetSkillInfo();
Console.WriteLine($"接口数量: {info.Functions.Count}");

// 3. 生成 SKILL.md
SkillDocumentGenerator.GenerateToFile(info, "SKILL.md");
```

### 项目结构

```
AiCsharpFunc.sln
src/
  AiSkillFramework/          # 核心框架类库
    Attributes/
      SkillAttribute.cs          # 标注 Skill 类
      SkillFunctionAttribute.cs  # 标注 Skill 接口方法
      SkillParameterAttribute.cs # 描述输入参数
      SkillOutputFieldAttribute.cs # 描述输出字段
    Models/
      SkillInfo.cs               # Skill 元数据模型
      SkillFunctionInfo.cs       # 接口元数据模型
      SkillParameterInfo.cs      # 参数元数据模型
      SkillOutputFieldInfo.cs    # 输出字段元数据模型
    Core/
      SkillBase.cs               # Skill 抽象基类
      SkillRegistry.cs           # Skill 注册与发现
      SkillDocumentGenerator.cs  # SKILL.md 文档生成器
samples/
  TushareSkillSample/        # Tushare 金融数据 Skill 示例
```

### 运行示例

```bash
dotnet run --project samples/TushareSkillSample/TushareSkillSample
```
