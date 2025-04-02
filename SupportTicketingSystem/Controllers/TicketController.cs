using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupportTicketingSystem.Data;
using System.Security.Claims;

namespace SupportTicketingSystem.Controllers
{
    public class TicketController : Controller
    {
        // Declare the ApplicationDbContext as a class field
        private readonly ApplicationDbContext _context;

        // Constructor that takes the ApplicationDbContext and injects it
        public TicketController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Your actions go here, for example:

        // GET: /Ticket/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Ticket/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Subject,Description,Team,Status")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Associate the ticket with the logged-in user
                ticket.CreatedAt = DateTime.UtcNow;

                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); // Redirect to home after creation
            }

            return View(ticket);
        }

        // GET: /Home/Index (for displaying tickets on the home screen)
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tickets = await _context.Tickets
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .Take(10)
                .ToListAsync();

            return View(tickets);
        }
    }
}