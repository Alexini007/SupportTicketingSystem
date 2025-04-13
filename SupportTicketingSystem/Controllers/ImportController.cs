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

            if (!ticketArray.IsValid(schema, out IList<string> errors))
            {
                ViewBag.Message = "Schema validation failed: " + string.Join("; ", errors);
                return View("~/Views/Import/Import.cshtml");
            }

            // Retrieve the current user's ID from claims.
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            int importedCount = 0;
            foreach (var item in ticketArray)
            {
                var ticket = new Ticket
                {
                    Subject = item["Subject"]?.ToString(),
                    Description = item["Description"]?.ToString(),
                    Team = item["Team"]?.ToString(),
                    Status = item["Status"]?.ToString(),
                    UserId = currentUserId, // assign the logged-in user's ID.
                    CreatedAt = DateTime.UtcNow // automatically assign date,time
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