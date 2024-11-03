#nullable disable

namespace EZcore.DAL
{
    public abstract class Record
    {
        public int Id { get; set; }
        public string Guid { get; set; }
        public virtual string Name { get; set; }
    }
}
