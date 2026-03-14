using AiSkillFramework.Attributes;
using AiSkillFramework.Core;

namespace TushareSkillSample.Skills;

/// <summary>
/// Tushare 金融数据 Skill 示例。
/// 演示如何使用 AiSkillFramework 快速封装外部数据接口。
///
/// 使用方式：
///   1. 继承 <see cref="SkillBase"/>
///   2. 在类上标注 [Skill("name", Description = "...")]
///   3. 在每个接口方法上标注 [SkillFunction]，并用 [SkillParameter] 描述入参
///   4. 调用 GetSkillInfo() 获取元数据，或用 SkillDocumentGenerator 生成 SKILL.md
/// </summary>
[Skill("tushare",
    Description = "tushare是一个财经数据接口包，拥有丰富的数据内容，如股票、基金、期货、数字货币等行情数据，" +
                  "公司财务、基金经理等基本面数据。该模块通过标准化API方式统一了数据资产的对外服务方式，" +
                  "以帮助有需要的技术用户更实时、简洁、轻量的使用相关数据。",
    Version = "1.0.0",
    Author = "tushare")]
public class TushareSkill : SkillBase
{
    // ------------------------------------------------------------------
    // 股票数据 / 基础数据
    // ------------------------------------------------------------------

    /// <summary>获取 A 股股票基础信息列表。</summary>
    [SkillFunction(
        ApiName = "stock_basic",
        Description = "获取基础信息数据，包括股票代码、名称、上市日期、退市日期等",
        Category = "股票数据/基础数据",
        Permission = "2000积分起")]
    [SkillParameter("ts_code",    Type = "str",  Required = false, Description = "TS股票代码")]
    [SkillParameter("name",       Type = "str",  Required = false, Description = "名称")]
    [SkillParameter("market",     Type = "str",  Required = false, Description = "市场类别（主板/创业板/科创板/CDR/北交所）")]
    [SkillParameter("list_status",Type = "str",  Required = false, Description = "上市状态 L上市 D退市 P暂停上市，默认L")]
    [SkillParameter("exchange",   Type = "str",  Required = false, Description = "交易所 SSE上交所 SZSE深交所 BSE北交所")]
    [SkillParameter("is_hs",      Type = "str",  Required = false, Description = "是否沪深港通标的，N否 H沪股通 S深股通")]
    [SkillOutputField("ts_code",     Type = "str",   DefaultDisplay = true,  Description = "TS代码")]
    [SkillOutputField("symbol",      Type = "str",   DefaultDisplay = true,  Description = "股票代码")]
    [SkillOutputField("name",        Type = "str",   DefaultDisplay = true,  Description = "股票名称")]
    [SkillOutputField("area",        Type = "str",   DefaultDisplay = true,  Description = "地域")]
    [SkillOutputField("industry",    Type = "str",   DefaultDisplay = true,  Description = "所属行业")]
    [SkillOutputField("list_date",   Type = "str",   DefaultDisplay = true,  Description = "上市日期")]
    [SkillOutputField("delist_date", Type = "str",   DefaultDisplay = false, Description = "退市日期")]
    [SkillOutputField("is_hs",       Type = "str",   DefaultDisplay = false, Description = "是否沪深港通标的")]
    public Task<string> GetStockBasicAsync(
        string? tsCode = null,
        string? name = null,
        string? market = null,
        string listStatus = "L",
        string? exchange = null,
        string? isHs = null)
    {
        // 实际项目中此处调用 Tushare HTTP API 并返回 JSON/CSV 结果。
        // 本示例仅演示框架用法，直接返回占位字符串。
        return Task.FromResult(
            $"[stock_basic] ts_code={tsCode}, list_status={listStatus} — 请替换为真实 Tushare API 调用");
    }

    // ------------------------------------------------------------------
    // 股票数据 / 行情数据
    // ------------------------------------------------------------------

    /// <summary>获取股票日线行情数据。</summary>
    [SkillFunction(
        ApiName = "daily",
        Description = "获取股票日线行情数据（OHLCV）",
        Category = "股票数据/行情数据",
        Permission = "2000积分起")]
    [SkillParameter("ts_code",    Type = "str",            Required = true,  Description = "股票代码（如 000001.SZ）")]
    [SkillParameter("trade_date", Type = "str(YYYYMMDD)",  Required = false, Description = "交易日期")]
    [SkillParameter("start_date", Type = "str(YYYYMMDD)",  Required = false, Description = "开始日期")]
    [SkillParameter("end_date",   Type = "str(YYYYMMDD)",  Required = false, Description = "结束日期")]
    [SkillOutputField("ts_code",     Type = "str",   DefaultDisplay = true,  Description = "股票代码")]
    [SkillOutputField("trade_date",  Type = "str",   DefaultDisplay = true,  Description = "交易日期")]
    [SkillOutputField("open",        Type = "float", DefaultDisplay = true,  Description = "开盘价")]
    [SkillOutputField("high",        Type = "float", DefaultDisplay = true,  Description = "最高价")]
    [SkillOutputField("low",         Type = "float", DefaultDisplay = true,  Description = "最低价")]
    [SkillOutputField("close",       Type = "float", DefaultDisplay = true,  Description = "收盘价")]
    [SkillOutputField("vol",         Type = "float", DefaultDisplay = true,  Description = "成交量（手）")]
    [SkillOutputField("amount",      Type = "float", DefaultDisplay = true,  Description = "成交额（千元）")]
    [SkillOutputField("pct_chg",     Type = "float", DefaultDisplay = true,  Description = "涨跌幅（%）")]
    public Task<string> GetDailyAsync(
        string tsCode,
        string? tradeDate = null,
        string? startDate = null,
        string? endDate = null)
    {
        RequireParameter(tsCode, nameof(tsCode));
        if (startDate is not null) RequireDateFormat(startDate, nameof(startDate));
        if (endDate is not null)   RequireDateFormat(endDate,   nameof(endDate));

        return Task.FromResult(
            $"[daily] ts_code={tsCode}, start={startDate}, end={endDate} — 请替换为真实 Tushare API 调用");
    }

    // ------------------------------------------------------------------
    // 公募基金
    // ------------------------------------------------------------------

    /// <summary>获取基金基础信息。</summary>
    [SkillFunction(
        ApiName = "fund_basic",
        Description = "获取公募基金数据列表，包含基金的基本信息",
        Category = "公募基金",
        Permission = "2000积分起")]
    [SkillParameter("market",  Type = "str", Required = false, Description = "交易市场 E上交所 O深交所")]
    [SkillParameter("status",  Type = "str", Required = false, Description = "存续状态 D摘牌 I发行 L上市")]
    [SkillOutputField("ts_code",     Type = "str", DefaultDisplay = true,  Description = "基金代码")]
    [SkillOutputField("fund_name",   Type = "str", DefaultDisplay = true,  Description = "简称")]
    [SkillOutputField("fund_type",   Type = "str", DefaultDisplay = true,  Description = "基金类型")]
    [SkillOutputField("found_date",  Type = "str", DefaultDisplay = true,  Description = "成立日期")]
    [SkillOutputField("delist_date", Type = "str", DefaultDisplay = false, Description = "退市日期")]
    public Task<string> GetFundBasicAsync(string? market = null, string? status = "L")
    {
        return Task.FromResult(
            $"[fund_basic] market={market}, status={status} — 请替换为真实 Tushare API 调用");
    }

    // ------------------------------------------------------------------
    // 宏观经济
    // ------------------------------------------------------------------

    /// <summary>获取国内生产总值（GDP）数据。</summary>
    [SkillFunction(
        ApiName = "cn_gdp",
        Description = "获取国内生产总值数据（季度）",
        Category = "宏观经济",
        Permission = "2000积分起")]
    [SkillParameter("start_q", Type = "str(YYYYQ)", Required = false, Description = "开始季度（如 2020Q1）")]
    [SkillParameter("end_q",   Type = "str(YYYYQ)", Required = false, Description = "结束季度")]
    [SkillOutputField("quarter",  Type = "str",   DefaultDisplay = true, Description = "季度")]
    [SkillOutputField("gdp",      Type = "float", DefaultDisplay = true, Description = "GDP累计值（亿元）")]
    [SkillOutputField("gdp_yoy",  Type = "float", DefaultDisplay = true, Description = "GDP同比增速（%）")]
    public Task<string> GetCnGdpAsync(string? startQ = null, string? endQ = null)
    {
        return Task.FromResult(
            $"[cn_gdp] start_q={startQ}, end_q={endQ} — 请替换为真实 Tushare API 调用");
    }
}
