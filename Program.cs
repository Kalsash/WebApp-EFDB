using Microsoft.Extensions.FileProviders;
using WebApp_Feed;
using WebApp_Landing;

namespace WebApp_EFDB
{
    public static class MiddlewareStaticExtension
    {
        public static IApplicationBuilder UseLocalStaticFiles(this IApplicationBuilder app, string projectFolder)
        {
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), projectFolder, "wwwroot"))
            });
            return app;
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add Feed Controllers
            builder.Services.AddFeedControllers();
            builder.Services.AddFeedDatabase(builder.Configuration.GetConnectionString("SQLiteConnection"));

            // Add Landing Pages
            builder.Services.AddLandingPages();



            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler();
            }

            // Add static files from feed
            app.UseLocalStaticFiles("WebApp-Feed");

            // Add static files from landing
            app.UseLocalStaticFiles("WebApp-Landing");

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapAreaControllerRoute(
                    name: "feed_area",
                    areaName: "Feed",
                    pattern: "/Feed/{controller=Home}/{action=Index}/{id?}");

            });

            app.Run();
        }
    }
}