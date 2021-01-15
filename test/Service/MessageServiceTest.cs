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

namespace MessageServiceTest
{
    public class MessageServiceTest
    {
        private readonly Mock<IMessageRepository> _messageRepository;
        private readonly List<Message> mockCollection = new List<Message>
        {
            new Message {SenderUsername = "onurkayabasi", ReceiverUsername = "testuser", Content = "Hi !"},
            new Message {SenderUsername = "testuser", ReceiverUsername = "onurkayabasi", Content = "Hi Onur !"},
            new Message {SenderUsername = "onurkayabasi", ReceiverUsername = "testuser", Content = "How are you ?"},
            new Message {SenderUsername = "testuser", ReceiverUsername = "onurkayabasi", Content = "I'm fine and you ?"},
            new Message {SenderUsername = "onurkayabasi", ReceiverUsername = "testuser", Content = "Thanks, I'm fine too"},
            new Message {SenderUsername = "testuser", ReceiverUsername = "testuser2", Content = "Hi !"},
        };

        public MessageServiceTest()
        {
            _messageRepository = new Mock<IMessageRepository>();
        }


        [Fact]
        public void GetMessages_When_Searched_Messages_Null_InRepo_Returns_Null()
        {
            _messageRepository.Setup(mr => mr.GetList(It.IsAny<Expression<Func<Message, bool>>>())).Returns(Task.FromResult(default(List<Message>)));

            MessageService messageService = new MessageService(_messageRepository.Object);

            var messages = messageService.GetMessages(m => m.SenderUsername == "onurkayabasi");

            Assert.Null(messages);
        }

        [Fact]
        public void GetMessages_Returns_Messages_InOrder_ByTime()
        {
            _messageRepository.Setup(mr => mr.GetList(It.IsAny<Expression<Func<Message, bool>>>())).Returns(Task.FromResult(mockCollection));

            MessageService messageService = new MessageService(_messageRepository.Object);

            var messages = messageService.GetMessages(m => m.SenderUsername == "onurkayabasi").Result;

            Assert.True(messages[0].TimeToSend < messages[1].TimeToSend);
        }

        [Fact]
        public void GetMessagesBetweenTwoUser_Returns_Just_Messages_From_User1_To_User2_And_From_User2_To_User1()
        {
            string userName1 = "onurkayabasi";
            string userName2 = "testuser";
            var expectedMessages = mockCollection.Where(m =>
                (m.SenderUsername == userName1 && m.ReceiverUsername == userName2)
                    ||
                (m.SenderUsername == userName2 && m.ReceiverUsername == userName1)).ToList();

            _messageRepository.Setup(mr => mr.GetList(It.IsAny<Expression<Func<Message, bool>>>())).Returns(Task.FromResult(expectedMessages));

            MessageService messageService = new MessageService(_messageRepository.Object);

            var messages = messageService.GetMessagesBetweenTwoUser("onurkayabasi", "testuser").Result;

            Assert.DoesNotContain(messages, m => m.ReceiverUsername == "testuser2");
        }

        [Fact]
        public void GetMessagesBetweenTwoUser_Returns_Messages_InOrder_ByTime()
        {
            string userName1 = "onurkayabasi";
            string userName2 = "testuser";
            var expectedMessages = mockCollection.Where(m =>
                (m.SenderUsername == userName1 && m.ReceiverUsername == userName2)
                    ||
                (m.SenderUsername == userName2 && m.ReceiverUsername == userName1)).ToList();

            _messageRepository.Setup(mr => mr.GetList(It.IsAny<Expression<Func<Message, bool>>>())).Returns(Task.FromResult(expectedMessages));

            MessageService messageService = new MessageService(_messageRepository.Object);

            var messages = messageService.GetMessagesBetweenTwoUser("onurkayabasi", "testuser").Result;

            Assert.True(messages[0].TimeToSend < messages[1].TimeToSend);
        }
    }
}