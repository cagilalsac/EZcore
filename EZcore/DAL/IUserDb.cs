using EZcore.DAL.Users;
using Microsoft.EntityFrameworkCore;

namespace EZcore.DAL
{
    public interface IUserDb
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
    }
}
