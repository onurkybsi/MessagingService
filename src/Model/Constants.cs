namespace MessagingService.Model {
  public static class Constants {
    public static class MessageHub {
      public const string AllUsers = "AllUsers";
      public static class Role {
        public const string Admin = "Admin";
        public const string User = "User";
      }
    }

    public static class JwtAuthService {
      public static class ErrorMessages {
        public const string NoUserExistsHasThisUsername = "NoUserExistsHasThisUsername";
        public const string PasswordIsNotCorrect = "PasswordIsNotCorrect";
      }
    }

    public static class ValidationMessages {
      public const string UserAlreadyExists = "UserAlreadyExists";
      public const string ValueCanNotBeNull = "ValueCanNotBeNull";
      public const string StringCanNotBeNullEmptyOrWhiteSpace = "StringCanNotBeNullEmptyOrWhiteSpace";
      public const string PasswordMustBeMoreThanFourCharacters = "PasswordMustBeMoreThanFourCharacters";
    }

    public static class MessageHubService {
      public static class ErrorMessages {
        public const string SaveMessageGroupInfoMessageCanNotCreate = "SaveMessageGroupInfoMessageCanNotCreate";
        public const string MessageGroupIdIsNullOrEmpty = "MessageGroupIdIsNullOrEmpty";
      }
    }
  }
}