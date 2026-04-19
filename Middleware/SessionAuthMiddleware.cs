namespace FlowerShop.Middleware;
public class SessionAuthMiddleware
{
    private readonly RequestDelegate _next;
    public SessionAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? string.Empty;
        if (IsAnonymousAllowed(path) || IsLoggedIn(context))
        {
            await _next(context);
            return;
        }
        if (RequiresAuthentication(path))
        {
            var returnUrl = context.Request.Path + context.Request.QueryString;
            var encoded = Uri.EscapeDataString(returnUrl);
            context.Response.Redirect($"/Account/Login?returnUrl={encoded}");
            return;
        }
        await _next(context);
    }
    private static bool IsLoggedIn(HttpContext context) => context.Session.GetInt32("UserId") is not null;
    private static bool IsAnonymousAllowed(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return true;
        }
        return path.StartsWith("/Flowers", StringComparison.OrdinalIgnoreCase)
               || path.StartsWith("/Bouquets", StringComparison.OrdinalIgnoreCase)
               || path.StartsWith("/Cart", StringComparison.OrdinalIgnoreCase)
               || path.StartsWith("/Auth/Login", StringComparison.OrdinalIgnoreCase)
               || path.StartsWith("/Auth/Register", StringComparison.OrdinalIgnoreCase)
               || path.StartsWith("/Account/Login", StringComparison.OrdinalIgnoreCase)
               || path.StartsWith("/Account/Register", StringComparison.OrdinalIgnoreCase)
               || path == "/"
               || path.StartsWith("/css", StringComparison.OrdinalIgnoreCase)
               || path.StartsWith("/js", StringComparison.OrdinalIgnoreCase)
               || path.StartsWith("/lib", StringComparison.OrdinalIgnoreCase)
               || path.StartsWith("/images", StringComparison.OrdinalIgnoreCase);
    }
    private static bool RequiresAuthentication(string path)
    {
        return path.StartsWith("/Orders/History", StringComparison.OrdinalIgnoreCase)
               || path.StartsWith("/Orders/Create", StringComparison.OrdinalIgnoreCase)
               || path.StartsWith("/Users", StringComparison.OrdinalIgnoreCase)
               || path.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase)
               || path.StartsWith("/Categories", StringComparison.OrdinalIgnoreCase)
               || path.StartsWith("/Florist", StringComparison.OrdinalIgnoreCase);
    }
}
