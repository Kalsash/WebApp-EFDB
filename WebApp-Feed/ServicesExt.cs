using Microsoft.EntityFrameworkCore;
using System;
using WebApp_Feed.Database;

namespace WebApp_Feed
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddFeedControllers(this IServiceCollection services)
        {
            services.AddControllersWithViews()
                .AddApplicationPart(typeof(Controllers.HomeController).Assembly)
                .AddRazorOptions(options =>
                {
                    options.AreaViewLocationFormats.Add("/Areas/{2}/Views/{1}/{0}.cshtml");
                });
            return services;
        }

        public static IServiceCollection AddFeedDatabase(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<GreenswampContext>(opt => opt.UseSqlite(connectionString));
            return services;
        }
    }
}