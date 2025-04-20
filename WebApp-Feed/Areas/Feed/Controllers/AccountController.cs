using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp_Feed.Areas.Feed.Models;
using WebApp_Feed.Database;
using WebApp_Feed.Models;

namespace WebApp_Feed.Areas.Feed.Controllers
{
    [Area("Feed")]
    public class AccountController : Controller
    {
        private readonly SignInManager<Auth> _signInManager;
        private readonly UserManager<Auth> _userManager;
        private readonly GreenswampContext _dbContext;
        public AccountController(UserManager<Auth> userManager, SignInManager<Auth> signInManager, GreenswampContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = "/") => View(new LoginViewModel { ReturnUrl = returnUrl });

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, false);
                if (result.Succeeded)
                    return LocalRedirect(model.ReturnUrl ?? "/");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register(string returnUrl = "/") => View(new RegisterViewModel { ReturnUrl = returnUrl });

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Проверяем, что Username и Email не пустые
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Email))
            {
                ModelState.AddModelError("", "Имя пользователя и Email обязательны.");
                return View(model);
            }

            // Проверяем, нет ли уже такого пользователя
            var existingUser = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Username == model.Username);

            if (existingUser != null)
            {
                ModelState.AddModelError("Username", "Пользователь с таким именем уже существует.");
                return View(model);
            }

            // Создаём User
            var userEntity = new User
            {
                Username = model.Username,
                DisplayName = model.Username, // Или другое значение
                Bio = "Новый пользователь",
                AvatarUrl = "https://i.pravatar.cc/100"
            };

            _dbContext.Users.Add(userEntity);
            await _dbContext.SaveChangesAsync(); // Получаем UserId

            // Создаём Auth (IdentityUser)
            var authUser = new Auth
            {
                UserName = model.Username,
                Email = model.Email,
                // Id (UserId) автоматически свяжется с userEntity.UserId
            };

            // Создаём пользователя в Identity
            var result = await _userManager.CreateAsync(authUser, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(authUser, isPersistent: true);
                return LocalRedirect(model.ReturnUrl ?? "/Feed");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string returnUrl = "/")
        {
            await _signInManager.SignOutAsync();
            return LocalRedirect(returnUrl);
        }
    }
}
