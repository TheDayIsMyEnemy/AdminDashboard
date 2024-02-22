using System.Text;

namespace BlazorTemplate.Domain.Extensions
{
    public static class StringExtensions
    {
        public static string ToBase64(this string value) =>
            Convert.ToBase64String(Encoding.UTF8.GetBytes(value));

        public static string FromBase64(this string value) =>
            Encoding.UTF8.GetString(Convert.FromBase64String(value));

        public static bool Includes(
            this string? source,
            string? value,
            StringComparison comparison = StringComparison.OrdinalIgnoreCase
        )
        {
            if (string.IsNullOrEmpty(source))
                return false;
            if (string.IsNullOrEmpty(value))
                return false;

            return source.Contains(value, comparison);
        }
    }
}
