namespace EZcore.DAL
{
    public interface ISoftDelete
    {
        public bool? IsDeleted { get; set; }
    }
}
