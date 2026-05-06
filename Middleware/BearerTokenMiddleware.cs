namespace MediaPortal.Api.Middleware;

public class BearerTokenMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _token;

    public BearerTokenMiddleware(RequestDelegate next, IConfiguration config)
    {
        _next = next;
        _token = config["ApiToken"]
            ?? throw new InvalidOperationException("ApiToken is not configured.");
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip auth check for health endpoint
        if (context.Request.Path.StartsWithSegments("/health"))
        {
            await _next(context);
            return;
        }

        var auth = context.Request.Headers.Authorization.ToString();
        if (!auth.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) ||
            auth["Bearer ".Length..] != _token)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { error = "Unauthorized" });
            return;
        }

        await _next(context);
    }
}
