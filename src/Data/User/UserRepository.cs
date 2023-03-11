using MessagingService.Common;

namespace MessagingService.Data.User {

  internal class UserRepository : IUserRepository {

    private static readonly UserMapper MAPPER = new UserMapper();

    private readonly MessagingServiceDbContext _dbContext;

    internal UserRepository(MessagingServiceDbContext dbContext) {
      _dbContext = dbContext;
    }

    public Service.User.User Save(Service.User.User user) {
      Assertions.NotNull(user, "user cannot be null!");

      UserEntity entityToSave = MAPPER.map(user);

      var savedEntity = _dbContext.Add(entityToSave);
      _dbContext.SaveChanges();

      return MAPPER.map(savedEntity.Entity);
    }

    public Service.User.User GetById(int id) {
      throw new System.NotImplementedException();
    }

  }

}