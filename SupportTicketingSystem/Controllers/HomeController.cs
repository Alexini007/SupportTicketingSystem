using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SupportTicketingSystem.Data;
using SupportTicketingSystem.Models;
using System.Diagnostics;

namespace SupportTicketingSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(
                ILogger<HomeController> logger,
                ApplicationDbContext context,
                UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            List<Ticket> recentTickets = new();

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                recentTickets = _context.Tickets
                    .OrderByDescending(t => t.CreatedAt)
                    .Take(10)
                    .ToList();
            }

            ViewBag.RecentTickets = recentTickets;

            return View();
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

        [AllowAnonymous]
        public IActionResult TicketSummary()
        {
            var summary = _context.Tickets
                .Where(t => t.Status == "new" || t.Status == "open")
                .GroupBy(t => t.Team)
                .Select(g => new TeamTicketSummaryViewModel
                {
                    TeamName = g.Key,
                    NewCount = g.Count(t => t.Status == "new"),
                    OpenCount = g.Count(t => t.Status == "open")
                })
                .ToList();
            return PartialView("_TicketSummary", summary);
        }
    }
}
