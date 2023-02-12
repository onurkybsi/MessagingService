using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MessagingService.Model;
using Microsoft.IdentityModel.Tokens;
using Constants = MessagingService.Model.Constants.JwtAuthService;

namespace MessagingService.Service {
  public class JwtAuthService : IAuthService {
    private readonly JwtAuthSettings _settings;
    private readonly IUserService _userService;

    public JwtAuthService(JwtAuthSettings settings, IUserService userService) {
      _settings = settings;
      _userService = userService;
    }

    public async Task<AuthResult> Authenticate(LoginModel loginModel) {
      User loggedInUser = await _userService.GetUserByUsername(loginModel.Username);
      bool loggedInUserNotExist = loggedInUser is null;
      if (loggedInUserNotExist)
        return CreateUnauthenticatedResult(Constants.ErrorMessages.NoUserExistsHasThisUsername);

      bool enteredPasswordVerified = VerifyUserPassword(loggedInUser.HashedPassword, loginModel.Password);
      if (!enteredPasswordVerified)
        return CreateUnauthenticatedResult(Constants.ErrorMessages.PasswordIsNotCorrect);

      string createdToken = CreateToken(loggedInUser);

      await _userService.UpdateUserTokenById(loggedInUser.Id, createdToken);

      return CreateAuthenticatedResult(createdToken);
    }

    private AuthResult CreateAuthenticatedResult(string token)
        => new AuthResult { IsAuthenticated = true, Token = token };

    private AuthResult CreateUnauthenticatedResult(string message)
        => new AuthResult { IsAuthenticated = false, Message = message };

    private bool VerifyUserPassword(string hashedUserPassword, string checkedPassword) {
      string userHash = hashedUserPassword.Split(EncryptionHelper.SaltPointer)[0];
      string userSalt = hashedUserPassword.Split(EncryptionHelper.SaltPointer)[1];

      bool userVerified = EncryptionHelper.VerifyHashed(checkedPassword, userSalt, userHash);

      return userVerified;
    }

    private string CreateToken(User user) {
      var tokenHandler = new JwtSecurityTokenHandler();

      var token = tokenHandler.CreateToken(CreateTokenDescriptor(user));
      string tokenAsString = tokenHandler.WriteToken(token);

      return tokenAsString;
    }

    private SecurityTokenDescriptor CreateTokenDescriptor(User user)
        => new SecurityTokenDescriptor {
          Subject = new ClaimsIdentity(new[]
                        {
                    new Claim("Id", user.Id),
                    new Claim(ClaimTypes.NameIdentifier, user.Username),
                    new Claim(ClaimTypes.Role, user.Role),
            }),

          Audience = _settings.Audience,
          Issuer = _settings.Issuer,
          Expires = DateTime.UtcNow.AddDays(1),
          SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecurityKey)), SecurityAlgorithms.HmacSha256Signature)
        };
  }
}