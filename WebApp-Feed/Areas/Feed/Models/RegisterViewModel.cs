using WebApp_Feed.Areas.Feed.Models;

namespace WebApp_Feed.Models
{
    public class RegisterViewModel
    {
        public string Username { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
