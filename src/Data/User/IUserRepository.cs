namespace MessagingService.Data.User {

  public interface IUserRepository {

    Service.User.User Save(MessagingService.Service.User.User user);

    Service.User.User GetById(int id);

  }

}