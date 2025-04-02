using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SupportTicketingSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Ticket> Tickets { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Tickets validation
            builder.Entity<Ticket>()
                .Property(t => t.Subject)
                .IsRequired();

            builder.Entity<Ticket>()
                .Property(t => t.Description)
                .IsRequired();

            builder.Entity<ApplicationUser>()
                .Property(u => u.Team)
                .IsRequired();

            builder.Entity<ApplicationUser>()
                .HasCheckConstraint("CK_Team", "Team IN ('Development', 'Support', 'Sales')");

            builder.Entity<ApplicationUser>()
                .HasIndex(u => u.Email)
                .IsUnique();

            builder.Entity<ApplicationUser>()
               .Property(u => u.FullName)
               .IsRequired();

            // Enforce valid teams
            builder.Entity<Ticket>()
                .HasCheckConstraint("CK_Ticket_Team", "Team IN ('Development', 'Support', 'Sales')");

            // Enforce valid status
            builder.Entity<Ticket>()
                .HasCheckConstraint("CK_Ticket_Status", "Status IN ('new', 'open', 'closed')");
        }
    }

    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "Full Name is required.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Team selection is required.")]
        public string Team { get; set; }
    }
}
