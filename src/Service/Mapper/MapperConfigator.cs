using AutoMapper;
using MessagingService.Infrastructure;
using MessagingService.Model;

namespace MessagingService.Service
{
    public static class MapperConfigurator
    {
        private static Mapper Mapper = new Mapper(new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<User, ConnectedUserInfo>()
                .ForMember(d => d.IsAdmin, opt => opt.MapFrom(s => s.Role == Constants.MessageHub.Role.Admin));

                cfg.CreateMap<ProcessResult<AuthResult>, AuthResult>()
                .ForMember(d => d.IsAuthenticated, opt => opt.MapFrom(u => u.IsSuccessful))
                .ForMember(d => d.Token, opt => opt.MapFrom(u => u.ReturnObject.Token));
            }
        ));

        public static TDest MapTo<TDest>(this object src)
        {
            return (TDest)Mapper.Map<TDest>(src);
        }
    }
}