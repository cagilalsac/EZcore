#nullable disable

using EZcore.Attributes;
using EZcore.DAL;
using System.Security.Claims;

namespace EZcore.Models
{
    public class UserModel : Model<User>
    {
        [DisplayName("User Name", "Kullanıcı Adı")]
        public string UserName => Record.UserName;

        [DisplayName("Password", "Şifre")]
        public string Password => Record.Password;

        [DisplayName("E-Mail", "E-Posta")]
        public string EMail => Record.EMail;

        [DisplayName("Status", "Durum")]
        public string IsActive => Record.IsActive ? "Active" : "Not Active";

        [DisplayName("Roles", "Roller")]
        public string Roles => string.Join(", ", Record.UserRoles?.Select(ur => ur.Role?.Name));

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
                        claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
                    }
                }
                return claims;
            }
        }

        [DisplayName("Confirm Password", "Şifre Onay")]
        public string ConfirmPassword { get; set; }
    }
}
