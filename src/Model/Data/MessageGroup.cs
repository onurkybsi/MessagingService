using System.Collections.Generic;
using MessagingService.Infrastructure;

namespace MessagingService.Model {
  public class MessageGroup : MongoDBEntity {
    public string GroupName { get; set; }
    public string AdminUsername { get; set; }
    public HashSet<string> UsernamesInGroup { get; set; }
  }
}