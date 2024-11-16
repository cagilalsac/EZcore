#nullable disable

using EZcore.Attributes;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EZcore.DAL
{
	public class User : Record, ISoftDelete, IModifiedBy
    {
        [NotMapped, Obsolete]
        public override string Name { get => base.Name; set => base.Name = value; }

        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string UserName { get; set; }

        [Required]
        [StringLength(15, MinimumLength = 3)]
        public string Password { get; set; }

        [StringLength(140, MinimumLength = 7)]
        public string EMail { get; set; }

        public bool IsActive { get; set; } = true;

		[JsonIgnore]
		public List<UserRole> UserRoles { get; private set; } = new List<UserRole>();

        [NotMapped]
        public List<int> Roles
        {
            get => UserRoles?.Select(ur => ur.RoleId).ToList();
            set => UserRoles = value?.Select(v => new UserRole() { RoleId = v }).ToList();
        }

        public bool? IsDeleted { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
