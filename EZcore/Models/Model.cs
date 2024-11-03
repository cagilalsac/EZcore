#nullable disable

using EZcore.DAL;

namespace EZcore.Models
{
    public abstract class Model<TEntity> where TEntity : Record, new()
    {
        public TEntity Record { get; set; } = new TEntity();
    }
}
