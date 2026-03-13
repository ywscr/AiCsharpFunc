using SkillService.Data;
using SkillService.Middleware;
using SkillService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// In-memory stores (singletons survive for the lifetime of the process)
builder.Services.AddSingleton<FaqStore>();
builder.Services.AddSingleton<NonceStore>();

// Business services
builder.Services.AddSingleton<IFaqService, FaqService>();
builder.Services.AddSingleton<IAuthService, AuthService>();

var app = builder.Build();

// Service-to-service auth guard for all /api/skill/* routes
app.UseMiddleware<SkillAuthMiddleware>();

// Liveness probe — no auth required
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapControllers();

app.Run();
