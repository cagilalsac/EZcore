#nullable disable

using EZcore.DAL;
using EZcore.DAL.Users;
using Microsoft.EntityFrameworkCore;

namespace BLL.DAL
{
    public class Db : DbContext, IDb, IUserDb
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<ProductStore> ProductStores { get; set; }

		public DbSet<User> Users { get; set; }
		public DbSet<Role> Roles { get; set; }
		public DbSet<UserRole> UserRoles { get; set; }

		public Db(DbContextOptions<Db> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasOne(p => p.Category).WithMany(c => c.Products).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<ProductStore>().HasOne(ps => ps.Product).WithMany(p => p.ProductStores).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<ProductStore>().HasOne(ps => ps.Store).WithMany(s => s.ProductStores).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
