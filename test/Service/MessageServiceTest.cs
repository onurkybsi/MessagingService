using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MessagingService.Data;
using MessagingService.Model;
using MessagingService.Service;
using Moq;
using Xunit;

namespace MessageServiceTest.Service
{
    public class MessageServiceTest
    {
        private readonly MessageService _messageService;

        private readonly Mock<IMessageRepository> _messageRepository;
        private readonly Mock<IMessageGroupRepository> _messageGroupRepository;
        private readonly Mock<IMessageHubService> _messageHubService;

        private readonly List<Message> mockMessageCollection = new List<Message>
        {
            new Message {SenderUsername = "onurkayabasi", ReceiverUsername = "testuser", Content = "Hi !"},
            new Message {SenderUsername = "testuser", ReceiverUsername = "onurkayabasi", Content = "Hi Onur !"},
            new Message {SenderUsername = "onurkayabasi", ReceiverUsername = "testuser", Content = "How are you ?"},
            new Message {SenderUsername = "testuser", ReceiverUsername = "onurkayabasi", Content = "I'm fine and you ?"},
            new Message {SenderUsername = "onurkayabasi", ReceiverUsername = "testuser", Content = "Thanks, I'm fine too"},
            new Message {SenderUsername = "testuser", ReceiverUsername = "testuser2", Content = "Hi !"},
        };

        private readonly List<MessageGroup> mockMessageGroupCollection = new List<MessageGroup>
        {
            new MessageGroup {GroupName = "grup1", AdminUsername = "admin1"},
            new MessageGroup {GroupName = "grup2", AdminUsername = "admin2"},
        };

        public MessageServiceTest()
        {
            _messageRepository = new Mock<IMessageRepository>();
            _messageGroupRepository = new Mock<IMessageGroupRepository>();
            _messageHubService = new Mock<IMessageHubService>();

            _messageService = new MessageService(_messageRepository.Object, _messageGroupRepository.Object, _messageHubService.Object);
        }

        [Fact]
        public void GetMessagesBetweenTwoUser_Returns_Just_Messages_From_User1_To_User2_And_From_User2_To_User1()
        {
            string userName1 = "onurkayabasi";
            string userName2 = "testuser";
            var expectedMessages = mockMessageCollection.Where(m =>
                (m.SenderUsername == userName1 && m.ReceiverUsername == userName2)
                    ||
                (m.SenderUsername == userName2 && m.ReceiverUsername == userName1)).ToList();

            Expression<Func<Message, bool>> getListExpression = m =>
                (m.SenderUsername == userName1 && m.ReceiverUsername == userName2)
                    ||
                (m.SenderUsername == userName2 && m.ReceiverUsername == userName1);

            _messageRepository.Setup(mr => mr.GetList(It.IsAny<Expression<Func<Message, bool>>>())).Returns(Task.FromResult(expectedMessages));

            var messages = _messageService.GetMessagesBetweenTwoUser("onurkayabasi", "testuser").Result;

            Assert.DoesNotContain(messages, m => m.ReceiverUsername == "testuser2");
        }

        [Fact]
        public void GetMessagesBetweenTwoUser_Returns_Messages_InOrder_ByTime()
        {
            string userName1 = "onurkayabasi";
            string userName2 = "testuser";
            var expectedMessages = mockMessageCollection.Where(m =>
                (m.SenderUsername == userName1 && m.ReceiverUsername == userName2)
                    ||
                (m.SenderUsername == userName2 && m.ReceiverUsername == userName1)).ToList();

            _messageRepository.Setup(mr => mr.GetList(It.IsAny<Expression<Func<Message, bool>>>())).Returns(Task.FromResult(expectedMessages));

            var messages = _messageService.GetMessagesBetweenTwoUser("onurkayabasi", "testuser").Result;

            Assert.True(messages[0].TimeToSend < messages[1].TimeToSend);
        }

        [Fact]
        public void SaveMessage_Calls_MessageRepository_Create()
        {
            _messageRepository.Setup(mr => mr.Create(It.Is<Message>(m => m.Id == mockMessageCollection[0].Id && m.Content == mockMessageCollection[0].Content)));

            Task.FromResult(_messageService.SaveMessage(mockMessageCollection[0]));

            _messageRepository.Verify(m => m.Create(It.Is<Message>(m => m.Id == mockMessageCollection[0].Id && m.Content == mockMessageCollection[0].Content)), Times.Once);
        }

        [Fact]
        public void SaveMessageGroup_When_TransactionType_IsInsert_Calls_MessageRepository_Create()
        {
            _messageGroupRepository.Setup(mr => mr.Create(It.Is<MessageGroup>(m => m.Id == mockMessageGroupCollection[0].Id && m.AdminUsername == mockMessageGroupCollection[0].AdminUsername)));

            Task.FromResult(_messageService.SaveMessageGroup(new MessageGroupSaveContext(mockMessageGroupCollection[0].AdminUsername, mockMessageGroupCollection[0].GroupName)));

            _messageGroupRepository.Verify(m => m.Create(It.Is<MessageGroup>(m => m.Id == mockMessageGroupCollection[0].Id && m.AdminUsername == mockMessageGroupCollection[0].AdminUsername)), Times.Once);
        }

        [Fact]
        public void SaveMessageGroup_When_TransactionType_IsUpdate_Calls_MessageRepository_FindAndUpdate()
        {
            _messageGroupRepository.Setup(mr => mr.FindAndUpdate(It.IsAny<Expression<Func<MessageGroup, bool>>>(), It.IsAny<Action<MessageGroup>>()));

            Task.FromResult(_messageService.SaveMessageGroup(new MessageGroupSaveContext(mockMessageGroupCollection[0].AdminUsername, mockMessageGroupCollection[0].Id, "updated", MessageGroupUpdateType.AdditionToGroup)));

            _messageGroupRepository.Verify(mr => mr.FindAndUpdate(It.IsAny<Expression<Func<MessageGroup, bool>>>(), It.IsAny<Action<MessageGroup>>()), Times.Once);
        }
    }
}