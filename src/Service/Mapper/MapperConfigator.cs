using AutoMapper;
using MessagingService.Model;

namespace MessagingService.Service
{
    public static class MapperConfigurator
    {
        private static Mapper Mapper = new Mapper(new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<User, ConnectedUserInfo>()
                .ForMember(cu => cu.IsAdmin, opt => opt.MapFrom(u => u.Role == Constants.MessageHub.Role.Admin));
            }
        ));

        public static TDest MapTo<TDest>(this object src)
        {
            return (TDest)Mapper.Map<TDest>(src);
        }
    }
}