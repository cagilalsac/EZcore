#nullable disable

namespace EZcore.Models.JsonWebToken
{
    public class Jwt
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
