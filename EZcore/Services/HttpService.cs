#nullable disable

using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Text;

namespace EZcore.Services
{
    public abstract class HttpServiceBase
    {
        public virtual string UserIdentityName => _httpContextAccessor.HttpContext?.User.Identity?.Name;

        private readonly IHttpContextAccessor _httpContextAccessor;

        protected HttpServiceBase(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void RemoveSession(string key)
        {
            _httpContextAccessor.HttpContext.Session.Remove(key);
        }

        public T GetSession<T>(string key) where T : class
        {
            var serializedBytes = _httpContextAccessor.HttpContext.Session.Get(key);
            if (serializedBytes is null)
                return null;
            var serializedInstance = Encoding.UTF8.GetString(serializedBytes);
            return JsonConvert.DeserializeObject<T>(serializedInstance);
        }

        public void SetSession<T>(string key, T instance) where T : class
        {
            var serializedInstance = JsonConvert.SerializeObject(instance);
            var serializedBytes = Encoding.UTF8.GetBytes(serializedInstance);
            _httpContextAccessor.HttpContext.Session.Set(key, serializedBytes);
        }
    }

    public class HttpService : HttpServiceBase
    {
        public HttpService(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }
    }
}
