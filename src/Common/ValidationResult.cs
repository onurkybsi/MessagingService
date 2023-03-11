using System.Collections.Generic;
using Newtonsoft.Json;

namespace MessagingService.Common {

  public class ValidationResult {

    private readonly Dictionary<string, List<string>> propertyFailures = new Dictionary<string, List<string>>();

    public void AddFailure(string propertyName, string failureMessage) {
      Assertions.NotBlank(propertyName, "propertyName cannot be blank!");
      Assertions.NotBlank(failureMessage, "failureMessage cannot be blank!");

      if (!propertyFailures.ContainsKey(propertyName)) {
        propertyFailures.Add(propertyName, new List<string>());
      }
      propertyFailures.GetValueOrDefault(propertyName).Add(failureMessage);
    }

    public bool IsValid() {
      return propertyFailures.Count <= 0;
    }

    public override string ToString() {
      return JsonConvert.SerializeObject(propertyFailures);
    }

  }

}