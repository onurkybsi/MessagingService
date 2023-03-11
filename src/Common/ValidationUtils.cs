using System.Text.RegularExpressions;

namespace MessagingService.Common {

  public static class ValidationUtils {

    private static readonly Regex EMAIL_REGEX = new Regex("^\\S+@\\S+\\.\\S+$");
    private static readonly Regex STRONG_PASSWORD_REGEX = new Regex("^\\S+@\\S+\\.\\S+$");


    public static bool IsEmail(string value) {
      if (string.IsNullOrWhiteSpace(value)) {
        return false;
      }
      if (!EMAIL_REGEX.IsMatch(value)) {
        return false;
      }
      return true;
    }

    public static bool IsStrongPassword(string value) {
      if (string.IsNullOrWhiteSpace(value)) {
        return false;
      }
      if (!STRONG_PASSWORD_REGEX.IsMatch(value)) {
        return false;
      }
      return true;
    }

  }

}