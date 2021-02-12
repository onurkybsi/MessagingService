using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MessagingService.Infrastructure;
using MessagingService.Model;
using Microsoft.IdentityModel.Tokens;
using Constants = MessagingService.Model.Constants.JwtAuthService;

namespace MessagingService.Service
{
    public class JwtAuthService : IAuthService
    {
        private readonly JwtAuthSettings _settings;
        private readonly IUserService _userService;

        public JwtAuthService(JwtAuthSettings settings, IUserService userService)
        {
            _settings = settings;
            _userService = userService;
        }

        public async Task<AuthResult> Authenticate(LoginModel loginModel)
        {
            var processorParams = await CreateAuthenticateProcessorParams(loginModel);

            await Infrastructure.Utility.ProcessorExecuter(processorParams.Context, processorParams.ProcessedResult, async (contex, processedResult) => await _userService.UpdateUser(contex.User),
                VerifyUser, CreateToken);

            return processorParams.ProcessedResult.MapTo<AuthResult>();
        }

        private async Task<(AuthenticateContext Context, ProcessResult<AuthResult> ProcessedResult)> CreateAuthenticateProcessorParams(LoginModel loginModel)
        {
            ProcessResult<AuthResult> result = new ProcessResult<AuthResult> { IsSuccessful = true, ReturnObject = new AuthResult() };
            AuthenticateContext contex = await CreateAuthenticateContext(loginModel, result);

            return (contex, result);
        }

        private async Task<AuthenticateContext> CreateAuthenticateContext(LoginModel loginModel, ProcessResult<AuthResult> result)
        {
            AuthenticateContext contex = new AuthenticateContext { LoginModel = loginModel };
            await GetUser(contex, result);

            return contex;
        }

        private async Task GetUser(AuthenticateContext context, ProcessResult<AuthResult> result)
        {
            var user = await _userService.GetUser(u => u.Username == context.LoginModel.Username);
            if (user is null)
            {
                result.IsSuccessful = false;
                result.Message = Constants.ErrorMessages.NoUserExistsHasThisEmail;
            }
            else
            {
                context.User = user;
            }
        }

        private void VerifyUser(AuthenticateContext context, ProcessResult<AuthResult> result)
        {
            string userHash = context.User.HashedPassword.Split(EncryptionHelper.SaltPointer)[0];
            string userSalt = context.User.HashedPassword.Split(EncryptionHelper.SaltPointer)[1];

            bool userVerified = EncryptionHelper.VerifyHashed(context.LoginModel.Password, userSalt, userHash);

            if (!userVerified)
            {
                result.IsSuccessful = false;
                result.Message = Constants.ErrorMessages.PasswordIsNotCorrect;
            }
        }

        private void CreateToken(AuthenticateContext context, ProcessResult<AuthResult> result)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", context.User.Id),
                    new Claim(ClaimTypes.NameIdentifier, context.User.Username),
                    new Claim(ClaimTypes.Role, context.User.Role),
                }),

                Audience = _settings.Audience,
                Issuer = _settings.Issuer,
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecurityKey)), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            string tokenAsString = tokenHandler.WriteToken(token);

            context.User.Token = tokenAsString;
            result.ReturnObject.Token = tokenAsString;
        }

        private class AuthenticateContext
        {
            public LoginModel LoginModel { get; set; }
            public User User { get; set; }
        }
    }
}