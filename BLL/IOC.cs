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
            builder.Services.AddScoped(typeof(Service<Category, CategoryModel>), typeof(CategoryService));
            builder.Services.AddScoped(typeof(Service<Store, StoreModel>), typeof(StoreService));
            builder.Services.AddScoped(typeof(Service<Product, ProductModel>), typeof(ProductService));

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
