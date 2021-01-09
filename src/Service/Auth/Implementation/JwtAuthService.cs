using MessagingService.Model;

namespace MessagingService.Service
{
    public class JwtAuthService : IAuthService
    {
        private JwtAuthSettings _settings;

        public JwtAuthService(JwtAuthSettings settings)
        {
            _settings = settings;
        }

        public AuthResult Authenticate(ILoginModel user)
        {
            throw new System.NotImplementedException();
        }
    }
}