using System.Collections.Generic;
using MessagingService.Model;

namespace MessagingService.Hubs {
  public static class MessageHubState {
    public static Dictionary<string, ConnectedUserInfo> ConnectedUsers = new Dictionary<string, ConnectedUserInfo>();

    /// <summary>
    /// When the second connection is connected with the existing username, the first connection is blacklisted.
    /// </summary>
    public static HashSet<string> BlackList { get; set; } = new HashSet<string>();
  }
}