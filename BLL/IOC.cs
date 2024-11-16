using BLL.DAL;
using BLL.Models;
using BLL.Services;
using EZcore;
using EZcore.DAL;
using EZcore.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BLL
{
    public static class IOC
    {
        public static void ConfigureBLL(this WebApplicationBuilder builder)
        {
            // AppSettings:
            var appSettings = new AppSettings(builder.Configuration);
            appSettings.Bind();

            // Inversion of Control for DbContext:
            builder.Services.AddDbContext<IDb, Db>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Db")));

            // Inversion of Control for Services:
            builder.Services.AddScoped<Service<Category, CategoryModel>, CategoryService>();
            builder.Services.AddScoped<Service<Store, StoreModel>, StoreService>();
            builder.Services.AddScoped<Service<Product, ProductModel>, ProductService>();

            // EZcore:
            builder.ConfigureEZcore();
        }

        public static void ConfigureBLL(this WebApplication application)
        {
            // EZcore:
            application.ConfigureEZcore();
        }
    }
}
