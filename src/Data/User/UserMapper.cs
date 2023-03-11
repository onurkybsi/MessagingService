using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace MessagingService.Data.User {

  class UserMapper {

    private static readonly Mapper MAPPER_INSTANCE = new Mapper(new MapperConfiguration(cfg => {
      cfg.CreateMap<Service.User.User, UserEntity>();
      cfg.CreateMap<UserEntity, Service.User.User>();
    }));

    public Service.User.User map(UserEntity entity) {
      return MAPPER_INSTANCE.Map<UserEntity, Service.User.User>(entity);
    }

    public UserEntity map(Service.User.User dto) {
      return MAPPER_INSTANCE.Map<Service.User.User, UserEntity>(dto);
    }

  }

}