using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using SupportTicketingSystem.Data;
using System.Security.Claims;

namespace SupportTicketingSystem.Controllers
{
    [Authorize]
    public class ImportController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ImportController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View("~/Views/Import/Import.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ViewBag.Message = "Please upload a valid JSON file.";
                return View("~/Views/Import/Import.cshtml");
            }

            string jsonData;
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                jsonData = await reader.ReadToEndAsync();
            }

            string schemaPath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "ticket-schema.json");

            if (!System.IO.File.Exists(schemaPath))
            {
                ViewBag.Message = "Schema file not found.";
                return View("~/Views/Import/Import.cshtml");
            }

            string schemaJson = System.IO.File.ReadAllText(schemaPath);
            JSchema schema = JSchema.Parse(schemaJson);

            JArray ticketArray;
            try
            {
                ticketArray = JArray.Parse(jsonData);
            }
            catch (JsonReaderException ex)
            {
                ViewBag.Message = "Invalid JSON format: " + ex.Message;
                return View("~/Views/Import/Import.cshtml");
            }

            if (!ticketArray.IsValid(schema, out IList<string> schemaErrors))
            {
                ViewBag.Message = "Schema validation failed: " + string.Join("; ", schemaErrors);
                return View("~/Views/Import/Import.cshtml");
            }

            bool hasEmptyFields = ticketArray.Any(t =>
                string.IsNullOrWhiteSpace(t["Subject"]?.ToString()) ||
                string.IsNullOrWhiteSpace(t["Description"]?.ToString()));

            if (hasEmptyFields)
            {
                ViewBag.Message = "One or more tickets have empty required fields (Subject or Description).";
                return View("~/Views/Import/Import.cshtml");
            }

            var validStatuses = new[] { "new", "open", "closed", "in_progress" };
            var validTeams = new[] { "support", "sales", "development" };

            var invalidStatusItems = ticketArray
                .Where(t => !validStatuses.Contains(t["Status"]?.ToString()?.Trim().ToLower()))
                .Select(t => t["Status"]?.ToString())
                .Distinct()
                .ToList();

            var invalidTeamItems = ticketArray
                .Where(t => !validTeams.Contains(t["Team"]?.ToString()?.Trim().ToLower()))
                .Select(t => t["Team"]?.ToString())
                .Distinct()
                .ToList();

            if (invalidStatusItems.Any() || invalidTeamItems.Any())
            {
                var errorMessages = new List<string>();
                if (invalidStatusItems.Any())
                    errorMessages.Add("Invalid status value(s): " + string.Join(", ", invalidStatusItems));
                if (invalidTeamItems.Any())
                    errorMessages.Add("Invalid team value(s): " + string.Join(", ", invalidTeamItems));

                ViewBag.Message = "Import failed. " + string.Join(" | ", errorMessages);
                return View("~/Views/Import/Import.cshtml");
            }

            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int importedCount = 0;

            foreach (var item in ticketArray)
            {
                var ticket = new Ticket
                {
                    Subject = item["Subject"]?.ToString(),
                    Description = item["Description"]?.ToString(),
                    Team = item["Team"]?.ToString()?.Trim().ToLower(),
                    Status = item["Status"]?.ToString()?.Trim().ToLower(),
                    UserId = currentUserId,
                    CreatedAt = DateTime.UtcNow
                };

                _db.Tickets.Add(ticket);
                importedCount++;
            }

            await _db.SaveChangesAsync();

            ViewBag.Message = $"{importedCount} ticket(s) successfully imported.";
            return View("~/Views/Import/Import.cshtml");
        }
    }
}
