using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp_Feed.Database;
using WebApp_Feed.Models;

namespace WebApp_Feed.Areas.Feed.Controllers
{
    [Area("Feed")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    //[Authorize] // Только авторизованные
    public class PostApiController : ControllerBase
    {
        private readonly GreenswampContext _context;
        private readonly UserManager<Auth> _userManager;

        public PostApiController(GreenswampContext context, UserManager<Auth> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // POST: /api/feed/postapi
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return BadRequest("Текст поста не может быть пустым");

            //var user = await _userManager.GetUserAsync(User);
            //if (user == null)
            //    return Unauthorized();

            var post = new Post
            {
                Content = content,
                CreatedAt = BitConverter.GetBytes(DateTime.UtcNow.ToBinary()),
                PostType = "post",
                UserId = 1
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Пост успешно добавлен",
                postId = post.PostId,
                content = post.Content
            });
        }
    }
}
