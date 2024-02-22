namespace BlazorTemplate.Server.Extensions
{
    public static class UriExtensions
    {
        public static Uri SetPort(this Uri uri, int port)
        {
            var uriBuilder = new UriBuilder(uri);
            uriBuilder.Port = port;
            return uriBuilder.Uri;
        }
    }
}
