namespace BlazorTemplate.Domain.Extensions
{
    public static class EnumerableExtensions
    {
        public static string Join(this IEnumerable<string> sequence, string separator = ", ") =>
            string.Join(separator, sequence);

        public static string Join<T>(this IEnumerable<T> sequence, string separator = ", ") =>
            string.Join(separator, sequence);
    }
}
