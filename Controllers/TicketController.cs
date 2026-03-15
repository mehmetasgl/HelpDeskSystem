using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MiniHelpDesk.Data;
using MiniHelpDesk.Models;
using MiniHelpDesk.ViewModels;

namespace MiniHelpDesk.Controllers
{
    [Authorize(Roles = "User")]
    public class TicketController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TicketController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

public async Task<IActionResult> Index(int? categoryId, string? searchTerm)
{
    var userId = _userManager.GetUserId(User);
    
    // Kullanıcının ticket'larını getir
    var query = _context.Tickets
        .Include(t => t.Category)
        .Where(t => t.UserId == userId);

    // Kategori filtresi
    if (categoryId.HasValue && categoryId.Value > 0)
    {
        query = query.Where(t => t.CategoryId == categoryId.Value);
    }

    // Arama filtresi 
    if (!string.IsNullOrWhiteSpace(searchTerm))
    {
        query = query.Where(t => 
            t.Title.Contains(searchTerm) || 
            t.Description.Contains(searchTerm));
    }

    var tickets = await query
        .OrderByDescending(t => t.CreatedAt)
        .ToListAsync();

    var totalTickets = await _context.Tickets.CountAsync(t => t.UserId == userId);
    var openTickets = await _context.Tickets.CountAsync(t => t.UserId == userId && t.Status == "Open");
    var closedTickets = await _context.Tickets.CountAsync(t => t.UserId == userId && t.Status == "Closed");

    var categories = await _context.Categories.ToListAsync();

    var viewModel = new TicketIndexViewModel
    {
        Tickets = tickets,
        TotalTickets = totalTickets,
        OpenTickets = openTickets,
        ClosedTickets = closedTickets,
        Categories = categories,
        SelectedCategoryId = categoryId,
        SearchTerm = searchTerm
    };

    return View(viewModel);
}

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            return View();
        }

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(CreateTicketViewModel model)
{
    if (ModelState.IsValid)
    {
        var userId = _userManager.GetUserId(User);

        var ticket = new Ticket
        {
            Title = model.Title,
            Description = model.Description,
            CategoryId = model.CategoryId,
            Priority = model.Priority, // YENİ
            UserId = userId!,
            Status = "Open",
            CreatedAt = DateTime.Now
        };

        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Ticket başarıyla oluşturuldu!";
        return RedirectToAction(nameof(Index));
    }

    ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
    return View(model);
}

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);

            var ticket = await _context.Tickets
                .Include(t => t.Category)
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

public async Task<IActionResult> Edit(int? id)
{
    if (id == null)
    {
        return NotFound();
    }

    var userId = _userManager.GetUserId(User);

    var ticket = await _context.Tickets
        .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

    if (ticket == null)
    {
        return NotFound();
    }

    if (ticket.Status == "Closed")
    {
        TempData["Error"] = "Kapalı ticket'lar düzenlenemez!";
        return RedirectToAction(nameof(Details), new { id = ticket.Id });
    }

    var viewModel = new EditTicketViewModel
    {
        Id = ticket.Id,
        Title = ticket.Title,
        Description = ticket.Description,
        CategoryId = ticket.CategoryId,
        Status = ticket.Status,
        Priority = ticket.Priority
    };

    ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", ticket.CategoryId);
    return View(viewModel);
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, EditTicketViewModel model)
{
    if (id != model.Id)
    {
        return NotFound();
    }

    if (ModelState.IsValid)
    {
        var userId = _userManager.GetUserId(User);

        var ticket = await _context.Tickets
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

        if (ticket == null)
        {
            return NotFound();
        }

        if (ticket.Status == "Closed")
        {
            TempData["Error"] = "Kapalı ticket'lar düzenlenemez!";
            return RedirectToAction(nameof(Details), new { id = ticket.Id });
        }

        ticket.Title = model.Title;
        ticket.Description = model.Description;
        ticket.CategoryId = model.CategoryId;
        ticket.Status = model.Status;
        ticket.Priority = model.Priority;
        ticket.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        TempData["Success"] = "Ticket başarıyla güncellendi!";
        return RedirectToAction(nameof(Details), new { id = ticket.Id });
    }

    ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", model.CategoryId);
    return View(model);
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> ToggleStatus(int id)
{
    var userId = _userManager.GetUserId(User);

    var ticket = await _context.Tickets
        .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

    if (ticket == null)
    {
        return NotFound();
    }

    if (ticket.Status == "Closed")
    {
        TempData["Error"] = "Kapalı ticket'ları tekrar açma yetkiniz yok. Lütfen admin ile iletişime geçin.";
        return RedirectToAction(nameof(Details), new { id = ticket.Id });
    }

    ticket.Status = "Closed";
    ticket.UpdatedAt = DateTime.Now;

    await _context.SaveChangesAsync();

    TempData["Success"] = "Ticket başarıyla kapatıldı!";
    return RedirectToAction(nameof(Details), new { id = ticket.Id });
}
    }
}