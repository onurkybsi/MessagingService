using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace MessagingService.Infrastructure {
  public class ControllerBaseExtra : ControllerBase {
    protected string GetCurrentUsername()
        => User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
  }
}