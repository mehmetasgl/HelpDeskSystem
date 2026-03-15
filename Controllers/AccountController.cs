using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MiniHelpDesk.Models;
using MiniHelpDesk.ViewModels;

namespace MiniHelpDesk.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult SelectLoginType()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");

                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("Index", "Ticket");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string? type)
        {
            ViewBag.LoginType = type; 
            return View();
        }

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Login(LoginViewModel model, string? type)
{
    if (ModelState.IsValid)
    {
        var result = await _signInManager.PasswordSignInAsync(
            model.UserName, 
            model.Password, 
            model.RememberMe, 
            lockoutOnFailure: false);

        if (result.Succeeded)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            var roles = await _userManager.GetRolesAsync(user!);
            bool isAdmin = roles.Contains("Admin");

            if (type == "admin" && !isAdmin)
            {
                // Admin girişine user giriş yapmaya çalışıyor
                await _signInManager.SignOutAsync();
                ModelState.AddModelError(string.Empty, "Bu hesap admin değil! Lütfen kullanıcı girişini kullanın.");
                ViewBag.LoginType = type;
                return View(model);
            }
            else if (type == "user" && isAdmin)
            {
                await _signInManager.SignOutAsync();
                ModelState.AddModelError(string.Empty, "Bu hesap admin! Lütfen admin girişini kullanın.");
                ViewBag.LoginType = type;
                return View(model);
            }

            if (isAdmin)
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                return RedirectToAction("Index", "Ticket");
            }
        }

        ModelState.AddModelError(string.Empty, "Geçersiz kullanıcı adı veya şifre.");
    }

    ViewBag.LoginType = type;
    return View(model);
}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("SelectLoginType", "Account"); 
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}