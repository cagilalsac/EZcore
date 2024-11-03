using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EZcore.DAL
{
    public interface IDb : IDisposable
    {
        public DbSet<TEntity> Set<TEntity>() where TEntity : class;
        public int SaveChanges();
        public ChangeTracker ChangeTracker { get; }
    }
}
