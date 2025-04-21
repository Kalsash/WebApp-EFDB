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

            // Check if username or email already exists
            if (await _userManager.FindByNameAsync(model.Username) != null)
            {
                ModelState.AddModelError("Username", "Username is already taken.");
                return View(model);
            }

            if (await _userManager.FindByEmailAsync(model.Email) != null)
            {
                ModelState.AddModelError("Email", "Email is already registered.");
                return View(model);
            }

            // First create the User entity
            var userEntity = new User
            {
                Username = model.Username,
                DisplayName = model.Username,
                Bio = "New user",
                AvatarUrl = "https://i.pravatar.cc/100",
                IsActive = true
            };

            _dbContext.Users.Add(userEntity);
            await _dbContext.SaveChangesAsync(); // This generates the UserId

            // Now create the Auth entity with the same ID
            var authUser = new Auth
            {
                Id = userEntity.UserId, // Explicitly set the ID to match User.UserId
                UserName = model.Username,
                Email = model.Email,
                User = userEntity // Set the navigation property
            };

            // Create the user in Identity
            var result = await _userManager.CreateAsync(authUser, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(authUser, isPersistent: true);
                return RedirectToAction("Index", "Home");   
            }

            // If Identity creation fails, clean up the User entity we created
            _dbContext.Users.Remove(userEntity);
            await _dbContext.SaveChangesAsync();

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
