#nullable disable

using EZcore.DAL;
using Microsoft.EntityFrameworkCore;

namespace BLL.DAL
{
    public class Db : DbContext, IDb
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<ProductStore> ProductStores { get; set; }

        public Db(DbContextOptions<Db> options) : base(options)
        {
        }
    }
}
