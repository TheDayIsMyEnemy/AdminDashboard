namespace BlazorTemplate.Domain
{
    public static class Constants
    {

    }

    public static class Roles
    {
        public const string Admin = "Admin";
        public const string SuperAdmin = "SuperAdmin";
        public const string CompanyManager = "CompanyManager";

        public static readonly IEnumerable<string> List = new[]
        {
            Admin,
            SuperAdmin,
            CompanyManager
        };
    }
}