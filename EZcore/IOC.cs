using EZcore.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace EZcore
{
    public static class IOC
    {
        public static void ConfigureEZcore(this WebApplicationBuilder builder)
        {
            // Session:
            builder.Services.AddSession(config => config.IdleTimeout = TimeSpan.FromMinutes(60));

            // Inversion of Control for HttpContext:
            builder.Services.AddHttpContextAccessor();

            // Inversion of Control for Services:
            builder.Services.AddSingleton<HttpServiceBase, HttpService>();
        }

        public static void ConfigureEZcore(this WebApplication application)
        {
            // Session:
            application.UseSession();
        }
    }
}
