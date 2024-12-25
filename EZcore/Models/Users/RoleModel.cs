#nullable disable

using EZcore.Attributes;
using EZcore.DAL.Users;

namespace EZcore.Models.Users
{
    public class RoleModel : Model<Role>
    {
        [DisplayName("Adı", "Name")]
        public string Name => Record.Name;

        [DisplayName("Kullanıcı Sayısı", "User Count")]
        public int UserCount => Record.UserRoles?.Count ?? 0;

        [DisplayName("Kullanıcılar", "Users")]
        public List<User> Users => Record.UserRoles?.Select(ur => ur.User).ToList();

        [DisplayName("Oluşturulma Tarihi", "Create Date")]
        public string CreateDate => Record.CreateDate.HasValue ?
            Record.CreateDate.Value.ToShortDateString() : string.Empty;

        [DisplayName("Oluşturan", "Created By")]
        public string CreatedBy => Record.CreatedBy;

        [DisplayName("Güncellenme Tarihi", "Update Date")]
        public string UpdateDate => Record.UpdateDate.HasValue ?
            Record.UpdateDate.Value.ToShortDateString() : string.Empty;

        [DisplayName("Güncelleyen", "Updated By")]
        public string UpdatedBy => Record.UpdatedBy;
    }
}
