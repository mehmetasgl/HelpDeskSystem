using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniHelpDesk.Data;
using MiniHelpDesk.Models;
using MiniHelpDesk.ViewModels;

namespace MiniHelpDesk.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public ProfileController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // GET: /Profile/Index
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var userId = user.Id;
            var totalTickets = await _context.Tickets.CountAsync(t => t.UserId == userId);
            var openTickets = await _context.Tickets.CountAsync(t => t.UserId == userId && t.Status == "Open");
            var closedTickets = await _context.Tickets.CountAsync(t => t.UserId == userId && t.Status == "Closed");

            var viewModel = new ProfileViewModel
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email,
                TotalTickets = totalTickets,
                OpenTickets = openTickets,
                ClosedTickets = closedTickets
            };

            return View(viewModel);
        }

        // GET: /Profile/Edit
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var viewModel = new EditProfileViewModel
            {
                UserName = user.UserName!,
                Email = user.Email
            };

            return View(viewModel);
        }

        // POST: /Profile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Kullanıcı adı değişti mi kontrol et
            if (user.UserName != model.UserName)
            {
                var setUserNameResult = await _userManager.SetUserNameAsync(user, model.UserName);
                if (!setUserNameResult.Succeeded)
                {
                    foreach (var error in setUserNameResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
            }

            // Email değişti mi kontrol et
            if (user.Email != model.Email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!setEmailResult.Succeeded)
                {
                    foreach (var error in setEmailResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
            }

            // Kullanıcıyı tekrar login yap (username değiştiyse)
            await _signInManager.RefreshSignInAsync(user);

            TempData["Success"] = "Profiliniz başarıyla güncellendi!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Profile/ChangePassword
        public IActionResult ChangePassword()
        {
            return View();
        }

        // POST: /Profile/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["Success"] = "Şifreniz başarıyla değiştirildi!";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                if (error.Code == "PasswordMismatch")
                {
                    ModelState.AddModelError(string.Empty, "Mevcut şifre yanlış!");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        // POST: /Profile/DeleteAccount
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount(string confirmUsername)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Kullanıcı adı doğrulaması
            if (confirmUsername != user.UserName)
            {
                TempData["Error"] = "Kullanıcı adı doğrulaması başarısız! Hesap silinemedi.";
                return RedirectToAction(nameof(Index));
            }

            // Kullanıcının ticket'larını sil
            var userTickets = await _context.Tickets.Where(t => t.UserId == user.Id).ToListAsync();
            _context.Tickets.RemoveRange(userTickets);
            await _context.SaveChangesAsync();

            // Kullanıcıyı sil
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                await _signInManager.SignOutAsync();
                TempData["Success"] = "Hesabınız başarıyla silindi!";
                return RedirectToAction("SelectLoginType", "Account");
            }

            TempData["Error"] = "Hesap silinirken bir hata oluştu!";
            return RedirectToAction(nameof(Index));
        }
    }
}