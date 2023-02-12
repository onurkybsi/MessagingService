using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MessagingService.Hubs;
using MessagingService.Model;
using MessagingService.Service;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace MessageServiceTest.Service {
  // TO-DO: Ayrı ayrı doğru çalışıyorlar fakat birlikte çalışmıyorlar check et !
  public class MessageHubServiceTest {
    private readonly MessageService _messageService;

    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<IHubContext<MessageHub>> _mockMessageHubContex;
    private readonly Mock<IGroupManager> _mockGroupManager;
    private readonly MessageHubService _messageHubService;
    private readonly Mock<ILogger<MessageHubService>> _mockMessageHubServiceLogger;

    private readonly List<MessageGroup> mockMessageGroupCollection = new List<MessageGroup>
    {
            new MessageGroup {GroupName = "grup1", AdminUsername = "admin1"},
            new MessageGroup {GroupName = "grup2", AdminUsername = "admin2"},
        };

    public MessageHubServiceTest() {
      _mockUserService = new Mock<IUserService>();

      _mockMessageHubContex = new Mock<IHubContext<MessageHub>>();
      _mockGroupManager = new Mock<IGroupManager>();
      _mockMessageHubContex.Setup(hc => hc.Groups).Returns(_mockGroupManager.Object);
      _mockMessageHubServiceLogger = new Mock<ILogger<MessageHubService>>();

      _messageHubService = new MessageHubService(_mockUserService.Object, _mockMessageHubContex.Object, _mockMessageHubServiceLogger.Object);
    }

    [Fact]
    public void SaveMessageGroup_When_TransactionType_Is_Insert_And_AdminUser_NotExists_In_MessageHubState_DoesntCall_AddToGroupAsync() {
      _mockGroupManager.Setup(gm => gm.AddToGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()));

      Task.FromResult(_messageHubService.SaveMessageGroup(new MessageGroupSaveContext("onurkayabasi", "testgroup")));

      _mockGroupManager.Verify(gm => gm.AddToGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public void SaveMessageGroup_When_TransactionType_Is_Insert_And_AdminUser_Exists_In_MessageHubState_Call_AddToGroupAsync() {
      MessageHubState.ConnectedUsers.Add("onurkayabasi", new ConnectedUserInfo { ConnectionId = "connectionid" });
      _mockGroupManager.Setup(gm => gm.AddToGroupAsync("connectionid", "testgroup", CancellationToken.None));

      Task.FromResult(_messageHubService.SaveMessageGroup(new MessageGroupSaveContext("onurkayabasi", "testgroup") { MessageGroupId = "testmessagegroupid" }));

      _mockGroupManager.Verify(gm => gm.AddToGroupAsync("connectionid", "testmessagegroupid", CancellationToken.None), Times.Once);
    }

    [Fact]
    public void SaveMessageGroup_When_TransactionType_Is_Update_And_AddedUer_NotExists_In_MessageHubState_DoesntCall_AddToGroupAsync() {
      Task.FromResult(_messageHubService.SaveMessageGroup(new MessageGroupSaveContext("admin", "testgroup", "onurkayabasi", MessageGroupUpdateType.AdditionToGroup)));

      _mockGroupManager.Verify(gm => gm.AddToGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public void SaveMessageGroup_When_TransactionType_Is_Update_And_AddedUser_Exists_In_MessageHubState_Call_AddToGroupAsync() {
      MessageHubState.ConnectedUsers.Add("onurkayabasi2", new ConnectedUserInfo { ConnectionId = "connectionid2" });
      _mockGroupManager.Setup(gm => gm.AddToGroupAsync("connectionid2", "testmessagegroupid", CancellationToken.None));

      Task.FromResult(_messageHubService.SaveMessageGroup(
          new MessageGroupSaveContext("admin", "testgroup2", "onurkayabasi2", MessageGroupUpdateType.AdditionToGroup) { MessageGroupId = "testmessagegroupid" }
      ));

      _mockGroupManager.Verify(gm => gm.AddToGroupAsync("connectionid2", "testmessagegroupid", CancellationToken.None), Times.Once);
    }
  }
}