#nullable disable

namespace EZcore.Models
{
    public class JwtModel
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
