using System.Collections.Generic;
using MessagingService.Action;
using MessagingService.Model;
using MessagingService.Service;
using Moq;
using Xunit;

namespace MessageServiceTest.Action {
  public class SaveMessageGroupActionTest {
    private readonly SaveMessageGroupAction _saveMessageGroupAction;

    private readonly Mock<IMessageService> _mockMessageService;
    private readonly Mock<IMessageHubService> _mockMessageHubService;

    public SaveMessageGroupActionTest() {
      _mockMessageService = new Mock<IMessageService>();
      _mockMessageHubService = new Mock<IMessageHubService>();

      _saveMessageGroupAction = new SaveMessageGroupAction(new List<ISaveMessageGroup>(), new List<ISaveMessageGroupAsync>
      {
                _mockMessageService.Object, _mockMessageHubService.Object
            });
    }

    [Fact]
    public void SaveMessageGroup_Call_Every_ISaveMessageGroup() {
      int calledMethodCount = 0;
      var messageGroupSaveContext = new MessageGroupSaveContext("admin", "testgroup");
      _mockMessageService.Setup(ms => ms.SaveMessageGroup(It.Is<MessageGroupSaveContext>(sc =>
          sc.ProcessorUsername == messageGroupSaveContext.ProcessorUsername && sc.TransactionType == TransactionType.Insert))).Callback(() => {
            calledMethodCount++;
          });
      _mockMessageHubService.Setup(ms => ms.SaveMessageGroup(It.Is<MessageGroupSaveContext>(sc =>
          sc.ProcessorUsername == messageGroupSaveContext.ProcessorUsername && sc.TransactionType == TransactionType.Insert))).Callback(() => {
            calledMethodCount++;
          });

      var actionResult = _saveMessageGroupAction.SaveMessageGroup(messageGroupSaveContext);

      Assert.Equal(2, calledMethodCount);
    }

    [Fact]
    public void SaveMessageGroup_When_One_Action_Throw_Exception_Other_Actions_Dont_Call() {
      int calledMethodCount = 0;
      var messageGroupSaveContext = new MessageGroupSaveContext("admin", "testgroup");
      _mockMessageService.Setup(ms => ms.SaveMessageGroup(It.Is<MessageGroupSaveContext>(sc =>
          sc.ProcessorUsername == messageGroupSaveContext.ProcessorUsername && sc.TransactionType == TransactionType.Insert))).Callback(() => {
            calledMethodCount++;
            throw new System.Exception();
          });
      _mockMessageHubService.Setup(ms => ms.SaveMessageGroup(It.Is<MessageGroupSaveContext>(sc =>
          sc.ProcessorUsername == messageGroupSaveContext.ProcessorUsername && sc.TransactionType == TransactionType.Insert))).Callback(() => {
            calledMethodCount++;
          });

      var actionResult = _saveMessageGroupAction.SaveMessageGroup(messageGroupSaveContext);

      Assert.Equal(1, calledMethodCount);
    }

    [Fact]
    public void SaveMessageGroup_When_OneOf_Actions_ThrownException_Return_IsSuccessful_False() {
      _mockMessageService.Setup(ms => ms.SaveMessageGroup(It.IsAny<MessageGroupSaveContext>()))
          .Throws(new System.Exception());

      var actionResult = _saveMessageGroupAction.SaveMessageGroup(new MessageGroupSaveContext("admin", "testgroup"));

      Assert.False(actionResult.IsSuccessful);
    }

    [Fact]
    public void SaveMessageGroup_When_OneOf_Actions_ThrownException_Return_ExceptionMessage_As_ResultMessage() {
      string errorMessage = "erroroccured";

      _mockMessageService.Setup(ms => ms.SaveMessageGroup(It.IsAny<MessageGroupSaveContext>()))
          .Throws(new System.Exception(errorMessage));

      var actionResult = _saveMessageGroupAction.SaveMessageGroup(new MessageGroupSaveContext("admin", "testgroup"));

      Assert.Equal(errorMessage, actionResult.Message);
    }
  }
}