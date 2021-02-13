// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using MessagingService.Data;
// using MessagingService.Model;
// using MessagingService.Service;
// using Moq;
// using Xunit;

// namespace MessageServiceTest
// {
//     public class UserServiceTest
//     {
//         private readonly Mock<IUserRepository> _userRepository;
//         private readonly List<User> mockUserCollection = new List<User>
//         {
//             new User {Username = "onurkayabasi", Role = Constants.MessageHub.Role.Admin},
//             new User {Username = "testuser", Role = Constants.MessageHub.Role.User},
//         };

//         public UserServiceTest()
//         {
//             _userRepository = new Mock<IUserRepository>();
//         }


//         [Fact]
//         public void IsAdmin_When_There_Is_No_Admin_With_InputUsername_Returns_False()
//         {
//             _userRepository.Setup(mr => mr.Get(u => u.Username == "testuser" && u.Role == Constants.MessageHub.Role.Admin)).Returns(Task.FromResult(mockUserCollection.Where(u => u.Username == "testuser" && u.Role == Constants.MessageHub.Role.Admin).FirstOrDefault()));

//             UserService messageService = new UserService(_userRepository.Object);

//             bool userIsAdmin = messageService.IsAdmin("testuser").Result;

//             Assert.False(userIsAdmin);
//         }
//     }
// }