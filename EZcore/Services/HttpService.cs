#nullable disable

using Microsoft.AspNetCore.Http;

namespace EZcore.Services
{
    public class HttpService : HttpServiceBase
    {
        public HttpService(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }
    }
}
