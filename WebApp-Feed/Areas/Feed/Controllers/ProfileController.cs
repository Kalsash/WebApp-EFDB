using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp_Feed.Database;
using WebApp_Feed.Models;

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

        // POST: //feed/postadd
        [HttpPost]
        [Route("profile/{username}")]
        public async Task<IActionResult> Post(string username, [FromBody] string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return BadRequest("Текст поста не может быть пустым");

            var user = _context.Users
                .Include(u => u.Posts)
                    .ThenInclude(p => p.Tags)
                .Include(u => u.Posts)
                    .ThenInclude(p => p.Interactions)
                .Include(u => u.Interactions)
                .FirstOrDefault(u => u.Username == username);

            var post = new Post
            {
                Content = content,
                CreatedAt = BitConverter.GetBytes(DateTime.UtcNow.ToBinary()),
                PostType = "post",
                UserId = user.UserId
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return Ok("Пост добавлен");
        }

    }
}
