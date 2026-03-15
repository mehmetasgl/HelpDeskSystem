using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MiniHelpDesk.Data;
using MiniHelpDesk.Models;
using MiniHelpDesk.ViewModels;
using MiniHelpDesk.Services;

namespace MiniHelpDesk.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(
            ApplicationDbContext context, 
            INotificationService notificationService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _notificationService = notificationService;
            _userManager = userManager;
        }

        // GET: /Admin/Index (Dashboard)
        public async Task<IActionResult> Index()
        {
            var totalTickets = await _context.Tickets.CountAsync();
            var openTickets = await _context.Tickets.CountAsync(t => t.Status == "Open");
            var closedTickets = await _context.Tickets.CountAsync(t => t.Status == "Closed");
            var totalUsers = await _context.Users.CountAsync();

            var viewModel = new AdminDashboardViewModel
            {
                TotalTickets = totalTickets,
                OpenTickets = openTickets,
                ClosedTickets = closedTickets,
                TotalUsers = totalUsers
            };

            return View(viewModel);
        }

        // GET: /Admin/Tickets
public async Task<IActionResult> Tickets(int? categoryId, string? answerStatus, string? searchTerm, string? priorityFilter) // priorityFilter eklendi
{
    var query = _context.Tickets
        .Include(t => t.Category)
        .Include(t => t.User)
        .AsQueryable();

    // Kategori filtresi
    if (categoryId.HasValue && categoryId.Value > 0)
    {
        query = query.Where(t => t.CategoryId == categoryId.Value);
    }

    // Cevap durumu filtresi
    if (!string.IsNullOrEmpty(answerStatus))
    {
        if (answerStatus == "answered")
        {
            query = query.Where(t => t.AdminAnswer != null);
        }
        else if (answerStatus == "unanswered")
        {
            query = query.Where(t => t.AdminAnswer == null);
        }
    }

    // Arama filtresi
    if (!string.IsNullOrWhiteSpace(searchTerm))
    {
        query = query.Where(t => 
            t.Title.Contains(searchTerm) || 
            t.Description.Contains(searchTerm) ||
            t.User.UserName.Contains(searchTerm));
    }

    // Öncelik filtresi (YENİ)
    if (!string.IsNullOrWhiteSpace(priorityFilter))
    {
        query = query.Where(t => t.Priority == priorityFilter);
    }

    // Önceliğe göre sırala: High > Medium > Low, sonra tarih
    var tickets = await query
        .OrderBy(t => t.Priority == "High" ? 0 : t.Priority == "Medium" ? 1 : 2)
        .ThenByDescending(t => t.CreatedAt)
        .ToListAsync();

    var categories = await _context.Categories.ToListAsync();

    var viewModel = new AdminTicketListViewModel
    {
        Tickets = tickets,
        Categories = categories,
        SelectedCategoryId = categoryId,
        SelectedAnswerStatus = answerStatus,
        SearchTerm = searchTerm,
        PriorityFilter = priorityFilter // YENİ
    };

    return View(viewModel);
}

        // GET: /Admin/TicketDetail/5
        public async Task<IActionResult> TicketDetail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.Category)
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: /Admin/AnswerTicket
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AnswerTicket(int id, string adminAnswer)
        {
            var ticket = await _context.Tickets.FindAsync(id);

            if (ticket == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(adminAnswer))
            {
                TempData["Error"] = "Cevap boş olamaz!";
                return RedirectToAction(nameof(TicketDetail), new { id });
            }

            if (adminAnswer.Length > 1000)
            {
                TempData["Error"] = "Cevap maksimum 1000 karakter olabilir!";
                return RedirectToAction(nameof(TicketDetail), new { id });
            }

            ticket.AdminAnswer = adminAnswer;
            ticket.AnsweredAt = DateTime.Now;
            ticket.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            // Bildirim oluştur (YENİ)
            await _notificationService.CreateNotificationAsync(
                ticket.UserId,
                $"'{ticket.Title}' başlıklı ticket'ınız cevaplandı!",
                $"/Ticket/Details/{ticket.Id}"
            );

            TempData["Success"] = "Ticket başarıyla cevaplandı ve kullanıcıya bildirim gönderildi!";
            return RedirectToAction(nameof(TicketDetail), new { id });
        }

        // POST: /Admin/ToggleTicketStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleTicketStatus(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);

            if (ticket == null)
            {
                return NotFound();
            }

            ticket.Status = ticket.Status == "Open" ? "Closed" : "Open";
            ticket.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Ticket durumu '{ticket.Status}' olarak güncellendi!";
            return RedirectToAction(nameof(TicketDetail), new { id });
        }

        // GET: /Admin/Categories
        public async Task<IActionResult> Categories()
        {
            var categories = await _context.Categories
                .Include(c => c.Tickets)
                .ToListAsync();

            return View(categories);
        }

        // GET: /Admin/CreateCategory
        public IActionResult CreateCategory()
        {
            return View();
        }

        // POST: /Admin/CreateCategory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(CategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var category = new Category
                {
                    Name = model.Name
                };

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Kategori başarıyla oluşturuldu!";
                return RedirectToAction(nameof(Categories));
            }

            return View(model);
        }

        // GET: /Admin/EditCategory/5
        public async Task<IActionResult> EditCategory(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            var viewModel = new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name
            };

            return View(viewModel);
        }

        // POST: /Admin/EditCategory/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(int id, CategoryViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var category = await _context.Categories.FindAsync(id);

                if (category == null)
                {
                    return NotFound();
                }

                category.Name = model.Name;
                await _context.SaveChangesAsync();

                TempData["Success"] = "Kategori başarıyla güncellendi!";
                return RedirectToAction(nameof(Categories));
            }

            return View(model);
        }

        // POST: /Admin/DeleteCategory/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Tickets)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            if (category.Tickets.Any())
            {
                TempData["Error"] = "Bu kategoriye ait ticket'lar olduğu için silinemez!";
                return RedirectToAction(nameof(Categories));
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Kategori başarıyla silindi!";
            return RedirectToAction(nameof(Categories));
        }
        // GET: /Admin/Users
        public async Task<IActionResult> Users(string? searchTerm, string? roleFilter)
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<UserManagementViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var totalTickets = await _context.Tickets.CountAsync(t => t.UserId == user.Id);
                var openTickets = await _context.Tickets.CountAsync(t => t.UserId == user.Id && t.Status == "Open");

                userViewModels.Add(new UserManagementViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName!,
                    Email = user.Email,
                    Roles = roles.ToList(),
                    TotalTickets = totalTickets,
                    OpenTickets = openTickets
                });
            }

            // Arama filtresi
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                userViewModels = userViewModels.Where(u => 
                    u.UserName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (u.Email != null && u.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            // Rol filtresi
            if (!string.IsNullOrWhiteSpace(roleFilter))
            {
                userViewModels = userViewModels.Where(u => u.Roles.Contains(roleFilter)).ToList();
            }

            ViewBag.SearchTerm = searchTerm;
            ViewBag.RoleFilter = roleFilter;

            return View(userViewModels);
        }

        // GET: /Admin/UserDetails/5
        public async Task<IActionResult> UserDetails(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var totalTickets = await _context.Tickets.CountAsync(t => t.UserId == user.Id);
            var openTickets = await _context.Tickets.CountAsync(t => t.UserId == user.Id && t.Status == "Open");
            var closedTickets = await _context.Tickets.CountAsync(t => t.UserId == user.Id && t.Status == "Closed");

            var recentTickets = await _context.Tickets
                .Include(t => t.Category)
                .Where(t => t.UserId == user.Id)
                .OrderByDescending(t => t.CreatedAt)
                .Take(10)
                .ToListAsync();

            var viewModel = new UserDetailsViewModel
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email,
                Roles = roles.ToList(),
                TotalTickets = totalTickets,
                OpenTickets = openTickets,
                ClosedTickets = closedTickets,
                RecentTickets = recentTickets
            };

            return View(viewModel);
        }

        // POST: /Admin/DeleteUser/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Kendi hesabını silmeye çalışıyorsa engelle
            var currentUserId = _userManager.GetUserId(User);
            if (user.Id == currentUserId)
            {
                TempData["Error"] = "Kendi hesabınızı silemezsiniz!";
                return RedirectToAction(nameof(Users));
            }

            // Admin kullanıcılarını silmeyi engelle
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Admin"))
            {
                TempData["Error"] = "Admin kullanıcıları silinemez!";
                return RedirectToAction(nameof(Users));
            }

            // Kullanıcının ticket'larını sil
            var userTickets = await _context.Tickets.Where(t => t.UserId == user.Id).ToListAsync();
            _context.Tickets.RemoveRange(userTickets);

            // Kullanıcının bildirimlerini sil
            var userNotifications = await _context.Notifications.Where(n => n.UserId == user.Id).ToListAsync();
            _context.Notifications.RemoveRange(userNotifications);

            await _context.SaveChangesAsync();

            // Kullanıcıyı sil
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                TempData["Success"] = $"{user.UserName} kullanıcısı başarıyla silindi!";
            }
            else
            {
                TempData["Error"] = "Kullanıcı silinirken bir hata oluştu!";
            }

            return RedirectToAction(nameof(Users));
        }
    }
}
