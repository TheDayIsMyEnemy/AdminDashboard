namespace BlazorTemplate.Infrastructure.Identity
{
    public static class Constants
    {
        public const string LoginPath = "/account/login";
        public const string LogoutPath = "/account/logout";
        public const string PermissionsClaimType = "permissions";
    }

    public enum Operation
    {
        Create,
        Read,
        Update,
        Delete,
    }

    public static class Policy
    {
        public const string ReadAccess = "ReadAccess";
        public const string WriteAccess = "WriteAccess";
        public const string EditAccess = "EditAccess";
        public const string DeleteAccess = "DeleteAccess";
    }

    public static class Roles
    {
        public const string Admin = "Administrator";

        public static readonly IEnumerable<string> List = new[]
        {
            Admin,
        };
    }
}
