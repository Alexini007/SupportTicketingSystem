using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupportTicketingSystem.Data;
using System.Security.Claims;

namespace SupportTicketingSystem.Controllers
{
    [Authorize]
    public class TicketController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly ILogger<TicketController> _logger;
        public TicketController(ApplicationDbContext context, ILogger<TicketController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: /Ticket/Create
        public IActionResult Create() => View();

        // POST: /Ticket/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ticket ticket)
        {
            ticket.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ticket.CreatedAt = DateTime.UtcNow;

            ModelState.Remove("User");
            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                _context.Add(ticket);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Ticket saved successfully.");
                return RedirectToAction(nameof(Index));
            }

            foreach (var entry in ModelState)
            {
                foreach (var error in entry.Value.Errors)
                {
                    _logger.LogWarning($"ModelState error in '{entry.Key}': {error.ErrorMessage}");
                }
            }

            return View(ticket);
        }

        // GET: /Ticket/Index
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); //get the logged in user's ID
            var currentUserTeam = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => u.Team) //get the team of the logged in user
                .FirstOrDefaultAsync();

            var tickets = await _context.Tickets
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return View(new TicketIndexViewModel
            {
                Tickets = tickets,
                CurrentUserTeam = currentUserTeam
            });
        }



        // GET: /Ticket/Edit/somenumberid
        public async Task<IActionResult> Edit(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            var currentUserTeam = await _context.Users
                .Where(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier))
                .Select(u => u.Team)
                .FirstOrDefaultAsync();

            if (ticket == null || ticket.Team != currentUserTeam)
                return Forbid();

            return View(ticket);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Ticket ticket)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserTeam = await _context.Users
                .Where(u => u.Id == currentUserId)
                .Select(u => u.Team)
                .FirstOrDefaultAsync();

            var existingTicket = await _context.Tickets.AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == ticket.Id);

            if (existingTicket == null || existingTicket.Team != currentUserTeam)
                return Forbid();

            ModelState.Remove("User");
            ModelState.Remove("UserId");

            if (!ModelState.IsValid)
                return View(ticket);

            ticket.UserId = existingTicket.UserId;
            ticket.CreatedAt = existingTicket.CreatedAt;

            _context.Update(ticket);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        // POST: /Ticket/Delete/somenumberint
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            var currentUserTeam = await _context.Users
                .Where(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier))
                .Select(u => u.Team)
                .FirstOrDefaultAsync();

            if (ticket == null || ticket.Team != currentUserTeam)
                return Forbid();

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
