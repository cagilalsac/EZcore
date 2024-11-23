#nullable disable

using EZcore.Attributes;
using EZcore.DAL.Users;
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

        [DisplayName("Durum", "Status")]
        public string IsActive => Record.IsActive ? "Aktif" : "Aktif değil";

        [DisplayName("Roller", "Roles")]
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

        [DisplayName("Şifre Onay", "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}
