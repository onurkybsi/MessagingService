using System;
using System.Collections.Generic;
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

        public async Task<List<User>> GetUsers(Expression<Func<User, bool>> filter)
            => await _userRepository.GetList(filter);

        public async Task CreateUser(User user)
            => await _userRepository.Create(user);

        public async Task UpdateUser(User user)
            => await _userRepository.Update(user);

        public async Task UpdateByUsername(string username, Action<User> updateDefinition)
            => await _userRepository.FindAndUpdate(f => f.Username == username, updateDefinition);

        public async Task<bool> IsAdmin(string userName)
            => (await _userRepository.Get(u => u.Username == userName && u.Role == Constants.MessageHub.Role.Admin)) != null;

        public async Task BlockUser(UserBlockingContext context)
            => await _userRepository.FindAndUpdate(u => u.Username == context.CurrentUsername, ud => ud.BlockedUsers.Add(context.BlockUserRequest.BlockedUsername));

        public async Task<HashSet<string>> GetBlockedUsersOfUser(string username)
            => await _userRepository.GetSpecifiedFieldByUsername<HashSet<string>>(username, u => u.BlockedUsers);
    }
}