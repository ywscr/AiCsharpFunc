// AiSkillFramework - AI Skill C# 封装框架
// 参考 https://github.com/waditu-tushare/skills 项目设计
//
// 快速使用：
// 1. 继承 SkillBase，用 [Skill] 标注类，用 [SkillFunction] 标注接口方法
// 2. 通过 SkillRegistry.BuildSkillInfo(typeof(YourSkill)) 获取元数据
// 3. 通过 SkillDocumentGenerator.GenerateToFile(info, "SKILL.md") 生成文档

global using AiSkillFramework.Attributes;
global using AiSkillFramework.Core;
global using AiSkillFramework.Models;
