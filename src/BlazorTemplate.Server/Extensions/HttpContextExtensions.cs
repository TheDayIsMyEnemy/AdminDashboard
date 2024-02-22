namespace BlazorTemplate.Server.Extensions
{
    public static class HttpContextExtensions
    {
        public static string? GetUserAgent(this HttpContext? httpContext)
             => httpContext?.Request?.Headers["User-Agent"];

        public static string? GetRemoteIpAddress(this HttpContext? httpContext)
            => httpContext?.Connection?.RemoteIpAddress?.ToString();
    }
}
