using BlazorTemplate.Domain;

namespace BlazorTemplate.UserInterface
{
    public static class AuthorizeConstants
    {
        public static readonly string AdminRoles = $"{Roles.SuperAdmin}, {Roles.Admin}";
    }

    public static class ResultMessages
    {
        public const string Created = "{0} has been created successfully.";
        public const string Updated = "{0} has been updated successfully.";
        public const string Deleted = "{0} has been deleted successfully.";
        public const string Error = "An error occurred while processing your request.";
        public const string NoPermissionToPerformThisAction = "You do not have permission to perform this action.";
    }
}
