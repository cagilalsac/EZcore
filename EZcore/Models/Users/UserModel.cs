#nullable disable

using EZcore.Attributes;
using EZcore.DAL.Users;
using EZcore.Extensions;
using System.Security.Claims;

namespace EZcore.Models.Users
{
    public class UserModel : Model<User>
    {
        [DisplayName("Kullanıcı Adı", "User Name")]
        public string UserName => Record.UserName;

        [DisplayName("Şifre", "Password")]
        public string Password => Record.Password;

        [DisplayName("E-Posta", "E-Mail")]
        public string EMail => Record.EMail;

        [DisplayName("Aktif Mi", "Is Active")]
        public string IsActive => Record.IsActive.ToHtml("<i class='bx bxs-user-check fs-3'></i>", "<i class='bx bxs-user-x fs-3'></i>");

        [DisplayName("Roller", "Roles")]
        public List<Role> Roles => Record.UserRoles?.Select(ur => ur.Role).ToList();

        public List<Claim> Claims
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Record.UserName))
                    return null;
                var claims = new List<Claim>()
                {
                    new Claim(nameof(Record.Id), Record.Id.ToString()),
                    new Claim(ClaimTypes.Name, Record.UserName)
                };
                if (Record.UserRoles is not null)
                {
                    foreach (var userRole in Record.UserRoles)
                    {
                        if (userRole.Role is not null)
                            claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
                    }
                }
                return claims;
            }
        }

        [DisplayName("Şifre Onay", "Confirm Password")]
        public string ConfirmPassword { get; set; }

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
