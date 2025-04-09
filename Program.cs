using Microsoft.Extensions.FileProviders;
using WebApp_Feed;
using WebApp_Landing;
using Serilog;

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
            builder.Services.AddScoped<ICsvHelperService, CsvHelperService>();

            // Add Landing Pages
            builder.Services.AddLandingPages();

            // Настройка Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()  // Поставил минимальный уровень логирования
                .WriteTo.Console()
                .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            builder.Host.UseSerilog(); //Serilog для логирования



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
            // Middleware для логирования запросов
            app.Use(async (context, next) =>
            {
                Log.Information("Request: {Method} {Path}", context.Request.Method, context.Request.Path);
                await next.Invoke(); // Переход к следующему middleware
                Log.Information("Response: {StatusCode} для {Method} {Path}", context.Response.StatusCode, context.Request.Method, context.Request.Path);
            });

            // Обработчик для 404 ошибок
            app.UseStatusCodePages(context =>
            {
                if (context.HttpContext.Response.StatusCode == StatusCodes.Status404NotFound)
                {
                    context.HttpContext.Response.Redirect("/404");
                }
                return Task.CompletedTask;
            });

            app.Run();
        }
    }
}