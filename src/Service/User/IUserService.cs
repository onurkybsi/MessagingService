using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MessagingService.Action;
using MessagingService.Model;

namespace MessagingService.Service.User {

  public interface IUserService {

    Task<User> SignUp(SignUpRequest request);

  }

}