using MessagingService.Data.User;
using Microsoft.EntityFrameworkCore;

namespace MessagingService.Data {

  internal class MessagingServiceDbContext : DbContext {

    public virtual DbSet<UserEntity> Users { get; set; }

  }

}