#nullable disable

namespace EZcore.DAL
{
    public interface IModifiedBy
    {
        public DateTime? CreateDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
