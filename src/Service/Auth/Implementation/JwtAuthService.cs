using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MessagingService.Model;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MessagingService.Service
{
    public class JwtAuthService : IAuthService
    {
        private readonly JwtAuthSettings _settings;
        private readonly IUserService _userService;

        public JwtAuthService(IOptions<JwtAuthSettings> settings, IUserService userService)
        {
            _settings = settings.Value;
            _userService = userService;
        }

        public async Task<AuthResult> Authenticate(ILoginModel login)
        {
            var user = await _userService.GetUser(u => u.Username == login.Username);
            if (user is null)
                return new AuthResult
                {
                    IsAuthenticated = false,
                    Message = "usernotexists"
                };

            if (!VerifyUser(login.Password, user.HashedPassword))
            {
                return new AuthResult
                {
                    IsAuthenticated = false,
                    Message = "passwordisnotcorrect"
                };
            }

            string createdToken = CreateToken(user);

            user.Token = createdToken;
            await _userService.UpdateUser(user);

            return new AuthResult
            {
                IsAuthenticated = true,
                Token = createdToken
            };
        }

        private bool VerifyUser(string loginPassword, string userHashedPassword)
        {
            string userHash = userHashedPassword.Split(EncryptionHelper.SaltPointer)[0];
            string userSalt = userHashedPassword.Split(EncryptionHelper.SaltPointer)[1];

            return !EncryptionHelper.VerifyHashed(loginPassword, userSalt, userHash) ? false : true;
        }

        private string CreateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("userId", user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username)
                }),

                Audience = _settings.Audience,
                Issuer = _settings.Issuer,
                Expires = DateTime.UtcNow.AddMinutes(200),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecurityKey)), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            string createdToken = tokenHandler.WriteToken(token);

            return createdToken;
        }
    }
}