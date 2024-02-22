namespace BlazorTemplate.Domain.Constants
{
    public static class ResultMessages
    {
        public static class Formats
        {
            public const string Created = "{0} has been created successfully.";
            public const string Updated = "{0} has been updated successfully.";
            public const string Deleted = "{0} has been deleted successfully.";
            public const string NotFound = "{0} not found.";
            public const string EntityPerformedActionOnEntity = "{0} has {1} {2}";
        }

        public const string InvalidAction = "The requested action is invalid.";
        public const string GenericError = "An error occurred while processing your request.";
        public const string NoPermissionToPerformThisAction = "You do not have permission to perform this action.";
    }
}