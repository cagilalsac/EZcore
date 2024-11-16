using EZcore.DAL;
using EZcore.Models;
using EZcore.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace EZcore
{
    public static class IOC
    {
        public static void ConfigureEZcore(this WebApplicationBuilder builder, int sessionExpirationInMinutes = 30, int authenticationCookieExpirationInMinutes = 60)
        {
            // JwtSettings:
            var jwtSettings = new JwtSettings(builder.Configuration);
            jwtSettings.Bind();

            // Authentication:
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, config =>
                {
                    config.LoginPath = "/Users/Login";
                    config.AccessDeniedPath = "/Users/Login";
                    config.SlidingExpiration = true;
                    config.ExpireTimeSpan = TimeSpan.FromMinutes(authenticationCookieExpirationInMinutes);
                })
                .AddJwtBearer(config =>
                {
                    config.TokenValidationParameters = new TokenValidationParameters()
                    {
                        IssuerSigningKey = JwtSettings.SigningKey,
                        ValidIssuer = JwtSettings.Issuer,
                        ValidAudience = JwtSettings.Audience,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true
                    };
                });

            // API CORS:
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod());
            });

            // API ModelState:
            builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

            // Session:
            builder.Services.AddSession(config => config.IdleTimeout = TimeSpan.FromMinutes(sessionExpirationInMinutes));

            // Inversion of Control for HttpContext:
            builder.Services.AddHttpContextAccessor();

            // Inversion of Control for Services:
            builder.Services.AddSingleton<HttpServiceBase, HttpService>();
            builder.Services.AddScoped<Service<User, UserModel>, UserService>();
        }

        public static void ConfigureEZcore(this WebApplication application)
        {
            // Authentication:
            application.UseAuthentication();

            // API CORS:
            application.UseCors();

            // Session:
            application.UseSession();
        }
    }
}
