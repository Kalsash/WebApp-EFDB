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
    [Authorize] // Только авторизованные
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

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            // Создаем новый пост
            var post = new Post
            {
                Content = content,
                CreatedAt = BitConverter.GetBytes(DateTime.UtcNow.ToBinary()),
                PostType = "post",
                UserId = user.Id,
                Tags = new List<Tag>() // Инициализация списка тегов
            };

            // Извлекаем хэштеги из содержимого поста
            var hashtags = WebApp_Feed.Models.Post.ExtractHashtags(content);

            // Обрабатываем каждый хэштег
            foreach (var tagName in hashtags)
            {
                // Добавляем тег и связываем его с постом
                var tag = AddTag(tagName);
                post.Tags.Add(tag); // Добавляем тег в список тегов поста
            }

            // Добавляем пост в контекст
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return Ok("Пост добавлен");
        }

        private Tag AddTag(string tagName)
        {
            var existingTag = _context.Tags
                .FirstOrDefault(t => t.TagName.ToLower() == tagName.ToLower());

            if (existingTag != null)
            {
                // Если тег существует, увеличиваем счетчик использования
                existingTag.UsageCount++;
                return existingTag; // Возвращаем существующий тег
            }
            else
            {
                // Если тег не существует, создаем новый
                var newTag = new Tag
                {
                    TagName = tagName,
                    CreatedAt = BitConverter.GetBytes(DateTime.UtcNow.Ticks),
                    UsageCount = 1
                };

                _context.Tags.Add(newTag);
                return newTag; // Возвращаем новый тег
            }
        }
    }
}
