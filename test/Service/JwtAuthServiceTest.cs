using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MessagingService.Model;
using MessagingService.Service;
using Moq;
using Xunit;

namespace MessageServiceTest
{
    public class JwtAuthServiceTest
    {
        private readonly Mock<IUserService> UserServiceMock;
        private JwtAuthSettings JwtSettings;
        private User TestUser = new User { Id = "testid", Username = "testusername", HashedPassword = EncryptionHelper.CreateHashed("testpassword"), Role = Constants.MessageHub.Role.User };

        public JwtAuthServiceTest()
        {
            UserServiceMock = new Mock<IUserService>();
            JwtSettings = new JwtAuthSettings
            {
                SecurityKey = "minimumSixteenCharactersSecurityKey",
                Issuer = "MessagingService",
                Audience = "someclients"
            };
        }

        [Fact]
        public void Authenticate_When_User_IsNot_Defined_Returns_IsAuthenticated_False()
        {
            UserServiceMock.Setup(us => us.GetUser(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(default(User)));

            var authService = new JwtAuthService(JwtSettings, UserServiceMock.Object);

            var authResult = authService.Authenticate(new LoginModel()).Result;

            Assert.False(authResult.IsAuthenticated);
        }

        [Fact]
        public void Authenticate_When_User_IsNot_Defined_Returns_Message_UserNotExists()
        {
            UserServiceMock.Setup(us => us.GetUser(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(default(User)));

            var authService = new JwtAuthService(JwtSettings, UserServiceMock.Object);

            var authResult = authService.Authenticate(new LoginModel()).Result;

            Assert.Equal(authResult.Message, Constants.ErrorMessages.UserNotExists);
        }

        [Fact]
        public void Authenticate_When_User_Password_IsNot_Correct_Returns_IsAuthenticated_False()
        {
            UserServiceMock.Setup(us => us.GetUser(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(TestUser));

            var authService = new JwtAuthService(JwtSettings, UserServiceMock.Object);

            var authResult = authService.Authenticate(new LoginModel
            {
                Username = "testusername",
                Password = "incorrect"
            }).Result;

            Assert.False(authResult.IsAuthenticated);
        }

        [Fact]
        public void Authenticate_When_User_Password_IsNot_Correct_Returns_Message_PasswordIsNotCorrect()
        {
            UserServiceMock.Setup(us => us.GetUser(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(TestUser));

            var authService = new JwtAuthService(JwtSettings, UserServiceMock.Object);

            var authResult = authService.Authenticate(new LoginModel
            {
                Username = "testusername",
                Password = "incorrect"
            }).Result;

            Assert.Equal(authResult.Message, Constants.ErrorMessages.PasswordIsNotCorrect);
        }

        [Fact]
        public void Authenticate_When_User_Defined_And_Password_Is_True_Then_User_BeUpdated_With_New_Token()
        {
            UserServiceMock.Setup(us => us.GetUser(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(TestUser));

            var authService = new JwtAuthService(JwtSettings, UserServiceMock.Object);

            var authResult = authService.Authenticate(new LoginModel
            {
                Username = TestUser.Username,
                Password = "testpassword"
            }).Result;

            TestUser.Token = authResult.Token;

            UserServiceMock.Verify(us => us.UpdateUser(TestUser), Times.Once);
        }

        [Fact]
        public void Authenticate_When_User_Defined_And_Password_Is_True_Returns_IsAuthenticated_True()
        {
            UserServiceMock.Setup(us => us.GetUser(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(TestUser));

            var authService = new JwtAuthService(JwtSettings, UserServiceMock.Object);

            var authResult = authService.Authenticate(new LoginModel
            {
                Username = TestUser.Username,
                Password = "testpassword"
            }).Result;

            Assert.True(authResult.IsAuthenticated);
        }

        [Fact]
        public void Authenticate_When_User_Defined_And_Password_Is_True_Returns_Token()
        {
            UserServiceMock.Setup(us => us.GetUser(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.FromResult(TestUser));

            var authService = new JwtAuthService(JwtSettings, UserServiceMock.Object);

            var authResult = authService.Authenticate(new LoginModel
            {
                Username = TestUser.Username,
                Password = "testpassword"
            }).Result;

            Assert.False(string.IsNullOrEmpty(authResult.Token));
        }
    }
}
