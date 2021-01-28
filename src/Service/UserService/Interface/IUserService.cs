using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MessagingService.Action;
using MessagingService.Model;

namespace MessagingService.Service
{
    public interface IUserService : IBlockUserAsync
    {
        Task<User> GetUser(Expression<Func<User, bool>> filter);
        Task<List<User>> GetUsers(Expression<Func<User, bool>> filter);
        Task CreateUser(User user);
        Task UpdateUser(User user);
        Task UpdateByUsername(string username, Action<User> updateDefinition);
        Task<bool> IsAdmin(string userName);
        Task<HashSet<string>> GetBlockedUsersOfUser(string username);
    }
}