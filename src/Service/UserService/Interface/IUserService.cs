using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MessagingService.Action;
using MessagingService.Model;

namespace MessagingService.Service
{
    public interface IUserService : IBlockUserAsync
    {
        Task<User> GetUser(Expression<Func<User, bool>> filter);
        Task CreateUser(User user);
        Task UpdateUser(User user);
        Task<ConnectedUserInfo> GetConnectedUserInfo(string username);
    }
}