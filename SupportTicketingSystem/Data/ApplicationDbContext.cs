using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SupportTicketingSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {
        }

        //public DbSet<Ticket> Tickets { get; set; }  // add  entity classes here
    }

    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string Team { get; set; }
    }
}
