#nullable disable

using EZcore.DAL;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace EZcore.Services
{
	public abstract class HttpServiceBase
    {
        public virtual string UserName => _httpContextAccessor.HttpContext?.User.Identity?.Name;
        public virtual int UserId => Convert.ToInt32(_httpContextAccessor.HttpContext?.User.Claims?.SingleOrDefault(claim => claim.Type == nameof(Record.Id)).Value);

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
            return JsonSerializer.Deserialize<T>(new ReadOnlySpan<byte>(serializedBytes));
        }

        public void SetSession<T>(string key, T instance) where T : class
        {
            var serializedBytes = JsonSerializer.SerializeToUtf8Bytes(instance);
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
