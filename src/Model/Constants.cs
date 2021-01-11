namespace MessagingService.Model
{
    public static class Constants
    {
        public static class MessageHub
        {
            public const string AllUsers = "AllUsers";
            public static class Role
            {
                public const string Admin = "Admin";
                public const string User = "User";
            }
        }

        public static class ErrorMessages
        {
            public const string UserNotExists = "UserNotExists";
            public const string PasswordIsNotCorrect = "PasswordIsNotCorrect";
        }
    }
}