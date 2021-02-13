using System.Threading.Tasks;
using MessagingService.Model;
using MessagingService.Service;
using Moq;
using Xunit;

namespace MessageServiceTest.Service
{
    public class JwtAuthServiceTest
    {
        JwtAuthService JwtAuthService;
        private JwtAuthSettings JwtSettings;
        private readonly Mock<IUserService> UserServiceMock;
        private User TestUser = new User
        {
            Id = "testid",
            Username = "testusername",
            HashedPassword = EncryptionHelper.CreateHashed("testpassword"),
            Role = Constants.MessageHub.Role.User
        };

        public JwtAuthServiceTest()
        {
            UserServiceMock = new Mock<IUserService>();
            JwtSettings = new JwtAuthSettings
            {
                SecurityKey = "minimumSixteenCharactersSecurityKey",
                Issuer = "MessagingService",
                Audience = "someclients"
            };
            JwtAuthService = new JwtAuthService(JwtSettings, UserServiceMock.Object);
        }

        [Fact]
        public void Authenticate_When_LoggedInUser_IsNotExist_Returns_IsAuthenticated_False()
        {
            UserServiceMock.Setup(us => us.GetUserByUsername(TestUser.Username)).Returns(Task.FromResult(default(User)));

            var authResult = JwtAuthService.Authenticate(new LoginModel()).Result;

            Assert.False(authResult.IsAuthenticated);
        }

        [Fact]
        public void Authenticate_When_LoggedInUser_IsNotExist_Returns_NoUserExistsHasThisEmail_Message()
        {
            UserServiceMock.Setup(us => us.GetUserByUsername(TestUser.Username)).Returns(Task.FromResult(default(User)));

            var authResult = JwtAuthService.Authenticate(new LoginModel()).Result;

            Assert.Equal(authResult.Message, Constants.JwtAuthService.ErrorMessages.NoUserExistsHasThisUsername);
        }

        [Fact]
        public void Authenticate_When_LoggedInUser_IsNotExist_Returns_Null_Token()
        {
            UserServiceMock.Setup(us => us.GetUserByUsername(TestUser.Username)).Returns(Task.FromResult(default(User)));

            var authResult = JwtAuthService.Authenticate(new LoginModel()).Result;

            Assert.Null(authResult.Token);
        }

        [Fact]
        public void Authenticate_When_LoggedInUser_Password_IsNotCorrect_Returns_IsAuthenticated_False()
        {
            UserServiceMock.Setup(us => us.GetUserByUsername(TestUser.Username)).Returns(Task.FromResult(TestUser));

            var authResult = JwtAuthService.Authenticate(new LoginModel
            {
                Username = TestUser.Username,
                Password = "incorrectpassword"
            }).Result;

            Assert.False(authResult.IsAuthenticated);
        }

        [Fact]
        public void Authenticate_When_LoggedInUser_Password_IsNotCorrect_Returns_PasswordIsNotCorrect_Message()
        {
            UserServiceMock.Setup(us => us.GetUserByUsername(TestUser.Username)).Returns(Task.FromResult(TestUser));

            var authResult = JwtAuthService.Authenticate(new LoginModel
            {
                Username = TestUser.Username,
                Password = "incorrect"
            }).Result;

            Assert.Equal(authResult.Message, Constants.JwtAuthService.ErrorMessages.PasswordIsNotCorrect);
        }

        [Fact]
        public void Authenticate_When_LoggedInUser_Password_IsNotCorrect_Returns_Null_Token()
        {
            UserServiceMock.Setup(us => us.GetUserByUsername(TestUser.Username)).Returns(Task.FromResult(TestUser));

            var authResult = JwtAuthService.Authenticate(new LoginModel
            {
                Username = TestUser.Username,
                Password = "incorrect"
            }).Result;

            Assert.Null(authResult.Token);
        }

        [Fact]
        public void Authenticate_When_LoggedInUser_Authenticated_Calls_UpdateUserTokenById()
        {
            UserServiceMock.Setup(us => us.GetUserByUsername(TestUser.Username)).Returns(Task.FromResult(TestUser));

            var authResult = JwtAuthService.Authenticate(new LoginModel
            {
                Username = TestUser.Username,
                Password = "testpassword"
            }).Result;

            UserServiceMock.Verify(us => us.UpdateUserTokenById(TestUser.Id, authResult.Token), Times.Once);
        }


        [Fact]
        public void Authenticate_When_LoggedInUser_Authenticated_Returns_IsAuthenticated_True()
        {
            UserServiceMock.Setup(us => us.GetUserByUsername(TestUser.Username)).Returns(Task.FromResult(TestUser));
            UserServiceMock.Setup(us => us.UpdateUserTokenById(TestUser.Id, It.IsAny<string>()));

            var authResult = JwtAuthService.Authenticate(new LoginModel
            {
                Username = TestUser.Username,
                Password = "testpassword"
            }).Result;

            Assert.True(authResult.IsAuthenticated);
        }

        [Fact]
        public void Authenticate_When_LoggedInUser_Authenticated_Returns_Null_Message()
        {
            UserServiceMock.Setup(us => us.GetUserByUsername(TestUser.Username)).Returns(Task.FromResult(TestUser));
            UserServiceMock.Setup(us => us.UpdateUserTokenById(TestUser.Id, It.IsAny<string>()));

            var authResult = JwtAuthService.Authenticate(new LoginModel
            {
                Username = TestUser.Username,
                Password = "testpassword"
            }).Result;

            Assert.Null(authResult.Message);
        }

        [Fact]
        public void Authenticate_When_LoggedInUser_Authenticated_Returns_NotNull_Token()
        {
            UserServiceMock.Setup(us => us.GetUserByUsername(TestUser.Username)).Returns(Task.FromResult(TestUser));
            UserServiceMock.Setup(us => us.UpdateUserTokenById(TestUser.Id, It.IsAny<string>()));

            var authResult = JwtAuthService.Authenticate(new LoginModel
            {
                Username = TestUser.Username,
                Password = "testpassword"
            }).Result;

            Assert.False(string.IsNullOrEmpty(authResult.Token));
        }
    }
}
