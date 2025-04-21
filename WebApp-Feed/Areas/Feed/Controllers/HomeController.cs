using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApp_Feed.Models;
using WebApp_Feed.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace WebApp_Feed.Controllers
{
    [Area("Feed")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly GreenswampContext _context;
        private readonly UserManager<Auth> _userManager;
        private readonly SignInManager<Auth> _signInManager;

        public HomeController(
            ILogger<HomeController> logger,
            GreenswampContext context,
            UserManager<Auth> userManager,
            SignInManager<Auth> signInManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new FeedViewModel
            {
                Posts = await _context.Posts
                    .Include(p => p.User)
                    .Include(p => p.Tags)
                    .Include(p => p.Interactions)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync(),
                Events = await _context.Events
                    .Include(e => e.Post)
                    .OrderBy(e => e.EventTime)
                    .ToListAsync(),
                IsAuthenticated = _signInManager.IsSignedIn(User), // Добавляем флаг аутентификации
                CurrentUser = _signInManager.IsSignedIn(User)
                    ? await _userManager.GetUserAsync(User)
                    : null
            };

            return View(viewModel);
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