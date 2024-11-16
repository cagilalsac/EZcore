#nullable disable

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace EZcore.Models
{
    public class JwtSettings : AppSettingsBase
    {
        public static string Audience { get; set; }
        public static string Issuer { get; set; }
        public static int ExpirationInMinutes { get; set; }
        public static string SecurityKey { get; set; }
        public static string SecurityAlgorithm { get; set; } = SecurityAlgorithms.HmacSha256Signature;
        public static SecurityKey SigningKey => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecurityKey));

        protected override string AppSettingsSection => "JwtSettings";

        public JwtSettings(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
