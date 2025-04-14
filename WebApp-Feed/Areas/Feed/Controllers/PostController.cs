using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp_Feed.Database;

namespace WebApp_Feed.Areas.Feed.Controllers
{
    [Area("Feed")]
    public class PostController : Controller
    {
        private readonly GreenswampContext _context;

        public PostController(GreenswampContext context)
        {
            _context = context;
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
    }
}
