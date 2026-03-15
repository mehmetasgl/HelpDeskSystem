using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MiniHelpDesk.Models;
using MiniHelpDesk.Services;

namespace MiniHelpDesk.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationController(INotificationService notificationService, UserManager<ApplicationUser> userManager)
        {
            _notificationService = notificationService;
            _userManager = userManager;
        }

        // GET: /Notification/Index
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var notifications = await _notificationService.GetUserNotificationsAsync(userId!);
            return View(notifications);
        }

        // POST: /Notification/MarkAsRead
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _notificationService.MarkAsReadAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // POST: /Notification/MarkAllAsRead
        [HttpPost]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = _userManager.GetUserId(User);
            await _notificationService.MarkAllAsReadAsync(userId!);
            return RedirectToAction(nameof(Index));
        }

        // POST: /Notification/Delete
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _notificationService.DeleteNotificationAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // API: Bildirim sayısını döndür (AJAX için)
        [HttpGet]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = _userManager.GetUserId(User);
            var count = await _notificationService.GetUnreadCountAsync(userId!);
            return Json(new { count });
        }
    }
}