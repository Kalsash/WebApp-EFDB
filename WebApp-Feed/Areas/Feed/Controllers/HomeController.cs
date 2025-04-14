using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApp_Feed.Models;
using WebApp_Feed.Database;
using Microsoft.EntityFrameworkCore;

namespace WebApp_Feed.Controllers
{
    [Area("Feed")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly GreenswampContext _context;

        public HomeController(ILogger<HomeController> logger, GreenswampContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var posts = _context.Posts
                .Include(p => p.User)
                .Include(p => p.Tags)
                .Include(p => p.Interactions)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            var events = _context.Events
                .Include(e => e.Post) 
                .OrderBy(e => e.EventTime) 
                .ToList();

            var combinedFeed = new FeedViewModel
            {
                Posts = posts,
                Events = events
            };

            return View(combinedFeed);
        }


        //public IActionResult Profile()
        //{
        //    return View();
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}