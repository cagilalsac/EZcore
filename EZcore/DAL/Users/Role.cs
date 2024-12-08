#nullable disable

using EZcore.Attributes;
using System.Text.Json.Serialization;

namespace EZcore.DAL.Users
{
    public class Role : Record, IName, ISoftDelete, IModifiedBy
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [JsonIgnore]
        public List<UserRole> UserRoles { get; private set; } = new List<UserRole>();

        public bool? IsDeleted { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
