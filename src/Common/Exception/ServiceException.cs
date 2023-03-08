using System;

namespace MessagingService.Common.Exception {

  public class ServiceException : ApplicationException {

    public bool DueToDataInvalidity { get; set; }
    public bool DueToNonexistentData { get; set; }

  }

}