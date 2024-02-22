namespace BlazorTemplate.Domain.Constants
{
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string CompanyManager = "CompanyManager";

        public static readonly IEnumerable<string> List = new[]
        {
            Admin,
            CompanyManager
        };
    }
}