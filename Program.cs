using MediaPortal.Api.Interfaces;
using MediaPortal.Api.Middleware;
using MediaPortal.Api.Repositories;
using MediaPortal.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        // camelCase JSON to match what the Vercel proxy expects
        o.JsonSerializerOptions.PropertyNamingPolicy =
            System.Text.Json.JsonNamingPolicy.CamelCase;
    });

builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<IRevenueRepository, RevenueRepository>();

// CORS — allow Vercel frontend and local dev
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000",
                "http://localhost:5173",
                "http://localhost:5174",
                "https://*.vercel.app")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors();

// Static Bearer token auth for all /api/* routes
app.UseMiddleware<BearerTokenMiddleware>();

// Health check — unauthenticated
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapControllers();

app.Run();
