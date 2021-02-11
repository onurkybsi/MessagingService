using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MessagingService.Data;
using MessagingService.Model;

namespace MessagingService.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> GetUser(Expression<Func<User, bool>> filter)
            => await _userRepository.Get(filter);

        public async Task CreateUser(User user)
            => await _userRepository.Create(user);

        public async Task UpdateUser(User user)
            => await _userRepository.Update(user);

        public async Task BlockUser(UserBlockingContext context)
            => await _userRepository.FindAndUpdate(u => u.Username == context.CurrentUsername, ud => ud.BlockedUsernames.Add(context.BlockUserRequest.BlockedUsername));

        public async Task<ConnectedUserInfo> GetConnectedUserInfo(string username)
            => (await _userRepository.Get(u => u.Username == username))?.MapTo<ConnectedUserInfo>();
    }
}