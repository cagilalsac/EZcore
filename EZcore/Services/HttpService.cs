#nullable disable

using EZcore.DAL;
using EZcore.Models.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Text.Json;

namespace EZcore.Services
{
    public abstract class HttpServiceBase
    {
        public string UserName => _httpContextAccessor.HttpContext?.User.Identity?.Name;
        public int UserId => Convert.ToInt32(_httpContextAccessor.HttpContext?.User.Claims?.SingleOrDefault(claim => claim.Type == nameof(Record.Id)).Value);

        private readonly IHttpContextAccessor _httpContextAccessor;

        protected HttpServiceBase(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public virtual void DeleteSession(string key)
        {
            _httpContextAccessor.HttpContext.Session.Remove(key);
        }

        public virtual T GetSession<T>(string key) where T : class
        {
            var serializedBytes = _httpContextAccessor.HttpContext.Session.Get(key);
            if (serializedBytes is null)
                return null;
            return JsonSerializer.Deserialize<T>(new ReadOnlySpan<byte>(serializedBytes));
        }

        public virtual void SetSession<T>(string key, T instance) where T : class
        {
            var serializedBytes = JsonSerializer.SerializeToUtf8Bytes(instance);
            _httpContextAccessor.HttpContext.Session.Set(key, serializedBytes);
        }

        public virtual async Task SignInAsync(UserModel user, bool isCookiePersistent = true)
        {
            var identity = new ClaimsIdentity(user.Claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var authenticationProperties = new AuthenticationProperties() { IsPersistent = isCookiePersistent };
            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authenticationProperties);
        }

        public virtual async Task SignOutAsync()
        {
            await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
		}

        public virtual void GetResponse(byte[] data, string fileNameWithExtension, string contentType)
        {
            if (data is not null && data.Length > 0)
            {
                _httpContextAccessor.HttpContext.Response.Headers.Clear();
                _httpContextAccessor.HttpContext.Response.Clear();
                _httpContextAccessor.HttpContext.Response.ContentType = contentType;
                _httpContextAccessor.HttpContext.Response.Headers.Append("content-length", data.Length.ToString());
                _httpContextAccessor.HttpContext.Response.Headers.Append("content-disposition", "attachment; filename=\"" + fileNameWithExtension + "\"");
                _httpContextAccessor.HttpContext.Response.Body.WriteAsync(data, 0, data.Length);
                _httpContextAccessor.HttpContext.Response.Body.Flush();
            }
        }

        public virtual void SetCookie(string key, string value, CookieOptions cookieOptions)
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Append(key, value, cookieOptions);
        }

        public virtual void SetCookie(string key, string value, bool isHttpOnly = true, int? expirationInDays = null)
        {
            var cookieOptions = new CookieOptions()
            {
                Expires = expirationInDays.HasValue ? 
                    DateTime.SpecifyKind(DateTime.Now.AddDays(expirationInDays.Value), DateTimeKind.Utc) : DateTimeOffset.MaxValue,
                HttpOnly = isHttpOnly
            };
            SetCookie(key, value, cookieOptions);
        }

        public virtual string GetCookie(string key)
        {
            return _httpContextAccessor.HttpContext.Request.Cookies[key];
        }

        public virtual void DeleteCookie(string key, bool isHttpOnly = true)
        {
            var cookieOptions = new CookieOptions()
            {
                Expires = DateTime.SpecifyKind(DateTime.Now.AddDays(-1), DateTimeKind.Utc),
                HttpOnly = isHttpOnly
            };
            SetCookie(key, string.Empty, cookieOptions);
        }
    }

    public class HttpService : HttpServiceBase
    {
        public HttpService(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }
    }
}
