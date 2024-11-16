#nullable disable

using System.Text.Json.Serialization;

namespace EZcore.DAL
{
	public class UserRole : Record
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }

		[JsonIgnore]
		public User User { get; set; }

		[JsonIgnore]
		public Role Role { get; set; }
    }
}
