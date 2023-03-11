using System;

namespace MessagingService.Common {

  public static class Assertions {

    public static void NotNull(object value, string message) {
      if (value is null) {
        throw new ArgumentNullException(message);
      }
    }

    public static void NotBlank(string value, string message) {
      if (string.IsNullOrWhiteSpace(value)) {
        throw new ArgumentNullException(message);
      }
    }

  }

}