using System.Collections.Generic;

namespace MessagingService.Common {

  public class ValidationResult {

    private readonly Dictionary<string, List<string>> propertyFailures = new Dictionary<string, List<string>>();

    public void AddFailure(string propertyName, string failureMessage) {
      // TODO: Do the assertions
      if (!propertyFailures.ContainsKey(propertyName)) {
        propertyFailures.Add(propertyName, new List<string>());
      }
      propertyFailures.GetValueOrDefault(propertyName).Add(failureMessage);
    }

    public bool IsValid() {
      return propertyFailures.Count <= 0;
    }

    public override string ToString() {
      // TODO: Return JSON representation
      return base.ToString();
    }

  }

}