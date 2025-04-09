using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp_Feed.Database;

namespace WebApp_Feed.Areas.Feed.Controllers
{
    [Area("Feed")]
    public class ProfileController : Controller
    {
        private readonly GreenswampContext _context;

        public ProfileController(GreenswampContext context)
        {
            _context = context;
        }

        [Route("profile/{username}")]
        public IActionResult Index(string username)
        {
            var user = _context.Users
                .Include(u => u.Posts)
                    .ThenInclude(p => p.Tags)
                .Include(u => u.Posts)
                    .ThenInclude(p => p.Interactions)
                .Include(u => u.Interactions)
                .FirstOrDefault(u => u.Username == username);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
    }
}
