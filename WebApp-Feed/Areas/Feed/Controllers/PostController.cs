using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp_Feed.Database;
using WebApp_Feed.Models;

namespace WebApp_Feed.Areas.Feed.Controllers
{
    [Area("Feed")]
    public class PostController : Controller
    {
        private readonly GreenswampContext _context;
        private readonly UserManager<Auth> _userManager; // Изменено на Auth

        public PostController(GreenswampContext context, UserManager<Auth> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Route("feed/post/{postId}")]
        public IActionResult Index(long postId)
        {
            var post = _context.Posts
                .Include(p => p.User)
                .Include(p => p.Tags)
                .Include(p => p.Interactions)
                    .ThenInclude(i => i.User)
                .Include(p => p.Event)
                .Include(p => p.ParentPost)
                .Include(p => p.InverseParentPost)
                .FirstOrDefault(p => p.PostId == postId);

            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: /feed/addreply
        [HttpPost]
        [Route("/feed/addreply")]
        public async Task<IActionResult> AddReply(long parentPostId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return BadRequest("Ответ не может быть пустым");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            // Создаем новый ответ
            var reply = new Post
            {
                Content = content,
                CreatedAt = BitConverter.GetBytes(DateTime.UtcNow.ToBinary()),
                PostType = "reply",
                UserId = user.Id,
                ParentPostId = parentPostId // Связывает ответ с родительским постом
            };

            _context.Posts.Add(reply);

            // Увеличиваем количество ответов в Interactions
            var interaction = new Interaction
            {
                UserId = user.Id,
                PostId = parentPostId,
                InteractionType = "comment", // Вы можете использовать "answer" или "reply" в зависимости от вашего проекта
                CreatedAt = BitConverter.GetBytes(DateTime.UtcNow.Ticks)
            };

            _context.Interactions.Add(interaction); // Добавляем новое взаимодействие

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { postId = parentPostId });
        }

        // POST: /feed/reribb
        [HttpPost]
        [Route("/feed/reribb")]
        public async Task<IActionResult> Reribb(long postId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var interaction = new Interaction
            {
                UserId = user.Id, // Используем свойство Id вашего класса Auth
                PostId = postId,
                InteractionType = "reribb",
                CreatedAt = BitConverter.GetBytes(DateTime.UtcNow.Ticks)
            };

            _context.Interactions.Add(interaction);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { postId = postId });
        }
    }
}