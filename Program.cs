using Microsoft.Extensions.FileProviders;
using WebApp_Feed;
using WebApp_Landing;
using Serilog;
using WebApp_Feed.Database;
using WebApp_Feed.Models;

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

    public static class DatabaseInitializer
    {
        public static byte[] DateTimeToByteArray(DateTime dateTime)
        {
            long dateTimeBinary = dateTime.ToBinary();
            return BitConverter.GetBytes(dateTimeBinary);
        }

        public static DateTime ByteArrayToDateTime(byte[] byteArray)
        {
            long dateTimeBinary = BitConverter.ToInt64(byteArray, 0);
            return DateTime.FromBinary(dateTimeBinary);
        }
        public static void SeedDatabase(GreenswampContext context)
        {
            // Проверяем, существует ли база данных и есть ли пользователи
            if (!context.Users.Any())
            {
                // Добавляем тестового пользователя
                var user = new User
                {
                    UserId = 1, // уникальный ID
                    Username = "testuser",
                    DisplayName = "Test User",
                    Bio = "This is a test user.",
                    AvatarUrl = "https://i.pravatar.cc/100?u=dorm23_frogs@pravatar.com"
                };
                context.Users.Add(user);
                context.SaveChanges();
            }

            // Добавляем тестовые посты, если их нет
            if (!context.Posts.Any())
            {
                var posts = new List<Post>
        {
            new Post
            {
                PostId = 1,
                UserId = 1,
                Content = "Hello, this is my first post!",
                CreatedAt = DateTimeToByteArray(DateTime.UtcNow),
                MediaType = "image", // Обязательно указываем корректное значение
                MediaUrl = "https://i.pravatar.cc/100?u=dorm23_frogs@pravatar.com",
                PostType = "text"
            },
            new Post
            {
                PostId = 2,
                UserId = 1,
                Content = "Check out this cool image!",
                CreatedAt = DateTimeToByteArray(DateTime.UtcNow.AddMinutes(-5)),
                MediaType = "image", // Корректное значение
                MediaUrl = "http://example.com/image.png",
                PostType = "image"
            }
        };

                context.Posts.AddRange(posts);
                context.SaveChanges();
            }

            // Добавляем тестовые события, если их нет
            if (!context.Events.Any())
            {
                var events = new List<Event>
        {
            new Event
            {
                EventId = 1,
                PostId = 1,
                EventTime = DateTimeToByteArray(DateTime.UtcNow.AddDays(1)),
                HostOrg = "Greenswamp Society",
                Location = "Pond Park",
                MaxCapacity = 50,
                RsvpCount = 0
            },
            new Event
            {
                EventId = 2,
                PostId = 2,
                EventTime = DateTimeToByteArray(DateTime.UtcNow.AddDays(2)),
                HostOrg = "Frog Fest",
                Location = "Lily Pad Plaza",
                MaxCapacity = 100,
                RsvpCount = 0
            }
        };

                context.Events.AddRange(events);
                context.SaveChanges();
            }
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

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<GreenswampContext>();
                DatabaseInitializer.SeedDatabase(dbContext);
            }

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