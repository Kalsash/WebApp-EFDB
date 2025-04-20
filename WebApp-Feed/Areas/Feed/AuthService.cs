using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using WebApp_Feed.Database;
using WebApp_Feed.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApp_Feed.Areas.Feed
{
    public class AuthService
    {
        private readonly GreenswampContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(GreenswampContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public static DateTime ByteArrayToDateTime(byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0)
                return DateTime.MinValue;

            try
            {
                long dateTimeBinary = BitConverter.ToInt64(byteArray, 0);
                return DateTime.FromBinary(dateTimeBinary);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        public static byte[] DateTimeToByteArray(DateTime dateTime)
        {
            long dateTimeBinary = dateTime.ToBinary();
            return BitConverter.GetBytes(dateTimeBinary);
        }

        public async Task<bool> LoginAsync(string username, string password, bool rememberMe)
        {
            var user = await _context.Users
                .Include(u => u.Auth)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null || user.Auth == null || !VerifyPassword(password, user.Auth.PasswordHash))
                return false;

            // Обновляем последний вход
            var authToUpdate = await _context.Auths.FindAsync(user.UserId);
            if (authToUpdate != null)
            {
                authToUpdate.LastLogin = DateTimeToByteArray(DateTime.UtcNow);
                await _context.SaveChangesAsync();
            }

            // Создаем claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("DisplayName", user.DisplayName ?? user.Username),
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = rememberMe,
                ExpiresUtc = rememberMe ? DateTimeOffset.UtcNow.AddDays(30) : null
            };

            await _httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                authProperties);

            return true;
        }

        public async Task LogoutAsync()
        {
            await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public async Task<bool> RegisterAsync(string username, string email, string password, string displayName)
        {
            if (await _context.Users.AnyAsync(u => u.Username == username))
                return false;

            var user = new User
            {
                Username = username,
                DisplayName = displayName,
                CreatedAt = DateTimeToByteArray(DateTime.UtcNow),
                IsActive = true,
                Auth = new Auth
                {
                    PasswordHash = HashPassword(password),
                    NormalizedUsername = username.ToUpperInvariant(),
                    SecurityStamp = Guid.NewGuid().ToString(),
                    LastLogin = null
                }
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return true;
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }
    }
}