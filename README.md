# AI Gateway + C# Skill Service Demo

> 演示如何通过 **OpenAI function calling** 让 AI 调用自定义 C# 技能服务（Skill Service），
> 对特定问题给出**确定性答案**，同时保护接口安全的完整示例。

---

## 目录

- [架构说明](#架构说明)
- [流程时序](#流程时序)
- [目录结构](#目录结构)
- [快速开始（本地运行）](#快速开始本地运行)
- [认证头说明](#认证头说明)
- [FAQ 匹配规则语法](#faq-匹配规则语法)
- [关键安全说明](#关键安全说明)
- [如何扩展更多 Skill](#如何扩展更多-skill)

---

## 架构说明

```
用户/客户端
    │  POST /ask  {"question":"..."}
    ▼
┌─────────────────────────────────┐
│        AI Gateway (Node.js)     │
│  server.js → openai-flow.js     │
│  • 注册 faq_match function tool │
│  • 调用 OpenAI Responses API    │
│  • 处理 function call           │
│  • 调用 C# Skill Service        │
└────────────────┬────────────────┘
                 │ HMAC-signed HTTP
                 ▼
┌─────────────────────────────────┐
│     C# Skill Service (.NET 8)   │
│  POST /api/skill/faq-match      │
│  • SkillAuthMiddleware 鉴权     │
│  • FaqService 模式匹配          │
│  • 返回 matched/answer/category │
└─────────────────────────────────┘
                 ▲
       内存中 FAQ 数据
    (seed-data/seed-faqs.json)
```

组件关系：

| 组件 | 技术 | 职责 |
|---|---|---|
| **AI Gateway** | Node.js + Express | 接收问题、与 OpenAI 交互、调用 Skill Service |
| **OpenAI Responses API** | OpenAI gpt-4o-mini | 理解用户意图、决定是否调用工具 |
| **C# Skill Service** | ASP.NET Core .NET 8 | 执行确定性 FAQ 匹配，返回权威答案 |
| **FAQ 存储** | 内存（JSON seed） | 存储规则和答案（可替换为数据库） |

---

## 流程时序

### 时序 1：Skill 命中 → 直接返回确定性答案

```
用户        AI Gateway      OpenAI          C# Skill
 │                │              │               │
 │── POST /ask ──►│              │               │
 │                │── responses.create ─────────►│  (发送 faq_match tool)
 │                │◄── function_call: faq_match ─│
 │                │── POST /api/skill/faq-match ─►│
 │                │◄── {matched:true, answer:...} ┤
 │                │  (跳过 OpenAI 改写)           │
 │◄── answer ─────│              │               │
```

当 `matched=true` 时，答案**直接**来自 C# Skill Service，不经过 OpenAI 改写，保证确定性输出。

### 时序 2：Skill 未命中 → 回退给模型

```
用户        AI Gateway      OpenAI          C# Skill
 │                │              │               │
 │── POST /ask ──►│              │               │
 │                │── responses.create ──────────►│
 │                │◄── function_call: faq_match ──┤
 │                │── POST /api/skill/faq-match ──►│
 │                │◄── {matched:false} ────────────┤
 │                │── responses.create (tool output)►│
 │                │◄── natural language answer ───┤
 │◄── answer ─────│              │               │
```

---

## 目录结构

```
.
├── SkillService/                  # C# ASP.NET Core .NET 8 服务
│   ├── Controllers/
│   │   └── FaqController.cs       # POST /api/skill/faq-match
│   ├── Middleware/
│   │   └── SkillAuthMiddleware.cs # 统一拦截 /api/skill/* 并验证签名
│   ├── Services/
│   │   ├── IFaqService.cs / FaqService.cs   # FAQ 匹配逻辑
│   │   └── IAuthService.cs / AuthService.cs # HMAC 签名验证
│   ├── Models/
│   │   ├── FaqItem.cs             # FAQ 数据模型
│   │   ├── FaqRequest.cs          # 请求模型
│   │   ├── FaqResponse.cs         # 响应模型
│   │   └── ClientInfo.cs          # 客户端凭据和权限模型
│   ├── Data/
│   │   ├── FaqStore.cs            # 内存 FAQ 存储（可替换为 DB）
│   │   └── NonceStore.cs          # Nonce 防重放存储
│   ├── seed-data/
│   │   └── seed-faqs.json         # FAQ 初始数据
│   ├── Program.cs
│   ├── appsettings.json
│   └── Dockerfile
│
├── ai-gateway/                    # Node.js AI Gateway
│   ├── server.js                  # Express HTTP 服务器
│   ├── openai-flow.js             # OpenAI Responses API 流程编排
│   ├── call-skill.js              # 调用 C# Skill Service（含 HMAC 签名）
│   ├── package.json
│   ├── Dockerfile
│   └── .env.example
│
├── docker-compose.yml             # 一键启动两个服务
├── .env.example                   # 根目录环境变量示例
└── README.md
```

---

## 快速开始（本地运行）

### 前提条件

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 20+](https://nodejs.org/)
- [Docker + Docker Compose](https://docs.docker.com/get-docker/)（可选，用于容器化运行）
- OpenAI API Key

---

### 方式一：使用 Docker Compose（推荐）

```bash
# 1. 复制环境变量文件并填入你的 OpenAI API Key
cp .env.example .env
# 编辑 .env，设置 OPENAI_API_KEY=sk-proj-...

# 2. 启动所有服务
docker-compose up --build

# 3. 测试
curl -X POST http://localhost:3000/ask \
  -H "Content-Type: application/json" \
  -d '{"question":"退款申请期限是多久？"}'
```

---

### 方式二：本地分别运行

**启动 C# Skill Service：**

```bash
cd SkillService
dotnet run
# 服务监听 http://localhost:5000
```

**启动 AI Gateway：**

```bash
cd ai-gateway
cp .env.example .env
# 编辑 .env，设置 OPENAI_API_KEY

npm install
npm start
# 网关监听 http://localhost:3000
```

**发送测试请求：**

```bash
# Skill 命中示例
curl -X POST http://localhost:3000/ask \
  -H "Content-Type: application/json" \
  -d '{"question":"退款申请期限是多久？"}'

# 预期响应（确定性答案）：
# {"source":"skill","faqId":"faq-001","category":"refund","answer":"退款申请需在签收后 7 天内提交。"}

# Skill 未命中示例（由 OpenAI 回答）
curl -X POST http://localhost:3000/ask \
  -H "Content-Type: application/json" \
  -d '{"question":"请介绍一下量子计算"}'
# {"source":"model","answer":"..."}
```

---

## 认证头说明

C# Skill Service 的所有 `/api/skill/*` 接口要求以下 HTTP 请求头：

| 请求头 | 说明 | 示例 |
|---|---|---|
| `X-Client-Id` | 注册的客户端 ID | `ai-gateway` |
| `X-Api-Key` | 客户端静态 API Key | `gw-api-key-change-me` |
| `X-Timestamp` | Unix 时间戳（秒，UTC） | `1741017600` |
| `X-Nonce` | 每次请求唯一的 UUID | `550e8400-e29b-41d4-a716-446655440000` |
| `X-Signature` | HMAC-SHA256 签名（小写十六进制） | `a3f2...` |

**签名计算方式（Node.js 示例）：**

```js
import { createHmac, randomUUID } from 'node:crypto';

const timestamp = String(Math.floor(Date.now() / 1000));
const nonce     = randomUUID();
const message   = `${timestamp}\n${nonce}\n${requestBody}`;
const signature = createHmac('sha256', HMAC_SECRET)
  .update(message)
  .digest('hex');
```

**签名计算方式（C# 示例）：**

```csharp
var message  = $"{timestamp}\n{nonce}\n{requestBody}";
var keyBytes = Encoding.UTF8.GetBytes(hmacSecret);
var msgBytes = Encoding.UTF8.GetBytes(message);
using var hmac = new HMACSHA256(keyBytes);
var signature = Convert.ToHexString(hmac.ComputeHash(msgBytes)).ToLowerInvariant();
```

---

## FAQ 匹配规则语法

`seed-data/seed-faqs.json` 中每条 FAQ 的 `pattern` 字段使用如下语法：

| 符号 | 含义 |
|---|---|
| `;` | AND（所有组均需匹配） |
| `\|` | OR（组内任意一个词匹配即可） |

**示例：**

```
退款|退货;期限|多久|几天
```

含义：问题中必须包含（`退款` 或 `退货`）**且**包含（`期限` 或 `多久` 或 `几天`）。

**添加新规则：** 直接编辑 `SkillService/seed-data/seed-faqs.json`，重启服务生效。

---

## 关键安全说明

| 机制 | 实现 | 作用 |
|---|---|---|
| **API Key** | `X-Api-Key` 头 + 固定时间比较 | 验证调用方身份，防止暴力猜测 |
| **HMAC-SHA256** | 签名覆盖 timestamp + nonce + body | 防止请求篡改和中间人攻击 |
| **Timestamp 有效期** | ±5 分钟窗口 | 限制重放攻击时间窗口 |
| **Nonce 防重放** | 内存 Set + 10 分钟保留窗口 | 防止同一合法请求被重放 |
| **固定时间比较** | `CryptographicOperations.FixedTimeEquals` | 防止时序攻击 |
| **Skill 权限控制** | `ClientInfo.AllowedSkills[]` 数组 | 每个客户端只能调用授权的 skill |
| **失败日志** | `ILogger.LogWarning` | 所有认证失败均记录日志，便于审计 |

> ⚠️ **生产环境注意事项：**
> - 将 `SKILL_API_KEY`、`SKILL_HMAC_SECRET` 替换为强随机值（建议 32+ 字节）。
> - 使用密钥管理服务（Azure Key Vault、AWS Secrets Manager 等）管理凭据，而非硬编码在配置文件中。
> - Nonce 存储在生产环境应使用 Redis 等分布式缓存，以支持多实例部署。
> - 在生产环境中为两个服务启用 HTTPS/TLS。

---

## 如何扩展更多 Skill

### 1. 在 C# Skill Service 中添加新接口

在 `FaqController.cs`（或新建 Controller）添加新接口：

```csharp
[HttpPost("order-status")]
public IActionResult OrderStatus([FromBody] OrderStatusRequest request)
{
    // 查询数据库 / 调用内部 API
    var status = _orderService.GetStatus(request.OrderId);
    return Ok(status);
}
```

在 `appsettings.json` 的客户端配置中添加新 skill 权限：

```json
"AllowedSkills": ["faq-match", "order-status"]
```

### 2. 在 AI Gateway 中注册新 function tool

在 `openai-flow.js` 中添加新工具定义并在 `tools` 数组中注册：

```js
const ORDER_TOOL = {
  type: 'function',
  name: 'order_status',
  description: '查询订单状态。当用户询问订单、物流、配送进度时必须调用此工具。',
  parameters: {
    type: 'object',
    properties: {
      order_id: { type: 'string', description: '订单号' },
    },
    required: ['order_id'],
  },
};
```

在 `call-skill.js` 中添加对应的调用函数：

```js
export async function callOrderStatus(orderId) {
  const body    = JSON.stringify({ orderId });
  const headers = buildAuthHeaders(body);
  const resp    = await fetch(`${SKILL_URL}/api/skill/order-status`, {
    method: 'POST', headers, body,
  });
  return await resp.json();
}
```

### 3. 扩展 FAQ 数据

无需修改代码，直接编辑 `SkillService/seed-data/seed-faqs.json` 添加新规则：

```json
{
  "id": "faq-008",
  "category": "warranty",
  "pattern": "保修|质保;多久|期限|几年",
  "answer": "产品享有 1 年官方质保，配件享有 3 个月质保。",
  "priority": 10,
  "isActive": true
}
```

---

## 许可证

MIT
