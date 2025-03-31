using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SupportTicketingSystem.Data;
using SupportTicketingSystem.Models;

[Authorize(Roles = "ORGANIZER")]
public class TicketController : Controller
{
    private readonly ApplicationDbContext _context;

    public TicketController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Ticket/Create
    public IActionResult Create()
    {
        // Get list of teams to populate the dropdown
        ViewData["TeamId"] = new SelectList(_context.Teams, "TeamId", "TeamName");
        return View();
    }

    // POST: Ticket/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Ticket ticket)
    {
        if (ModelState.IsValid)
        {
            ticket.CreatedByUserID = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            // If validation fails, repopulate the dropdown
            ViewData["TeamId"] = new SelectList(_context.Teams, "TeamId", "TeamName", ticket.TeamId);
            return View(ticket);
        }

        // Other actions like Edit, Delete, etc.
        _context.Add(ticket);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
