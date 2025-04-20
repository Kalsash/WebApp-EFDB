using Microsoft.AspNetCore.Mvc;

namespace WebApp_Feed.Areas.Feed.Controllers
{
    [Area("Feed")]
    public class AccountController : Controller
    {
        private readonly AuthService _authService;

        public AccountController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        [Route("account/login")]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, bool rememberMe, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (await _authService.LoginAsync(username, password, rememberMe))
            {
                return LocalRedirect(returnUrl ?? "/");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View();
        }

        [HttpGet]
        [Route("account/register")]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string username, string email, string password, string displayName, string returnUrl = null)
        {
            if (await _authService.RegisterAsync(username, email, password, displayName))
            {
                await _authService.LoginAsync(username, password, false);
                return LocalRedirect(returnUrl ?? "/");
            }

            ModelState.AddModelError(string.Empty, "Registration failed. Username may already exist.");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await _authService.LogoutAsync();
            return LocalRedirect(returnUrl ?? "/");
        }
    }
}
