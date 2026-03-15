using Microsoft.AspNetCore.Mvc;
using MiniHelpDesk.Models;
using System.Diagnostics;

namespace MiniHelpDesk.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
{
    // Giriş yapmamışsa SelectLoginType'a yönlendir
    if (!User.Identity?.IsAuthenticated ?? true)
    {
        return RedirectToAction("SelectLoginType", "Account");
    }

    // Giriş yapmışsa rolüne göre yönlendir
    if (User.IsInRole("Admin"))
    {
        return RedirectToAction("Index", "Admin");
    }
    else
    {
        return RedirectToAction("Index", "Ticket");
    }
}

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}