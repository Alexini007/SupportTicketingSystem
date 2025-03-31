using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SupportTicketingSystem.Models;
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
        public DbSet<TicketStatus> TicketStatuses { get; set; }
        public DbSet<TicketPriority> TicketPriorities { get; set; }
        public DbSet<TicketComment> TicketComments { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Team> Teams { get; set; }  // Add this line to include Teams


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

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

            // Adjust cascading delete behavior to avoid conflicts
            builder.Entity<Ticket>()
                .HasOne(t => t.Team)
                .WithMany(t => t.Tickets)
                .HasForeignKey(t => t.TeamId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Ticket>()
                .HasOne(t => t.CreatedByUser)
                .WithMany()
                .HasForeignKey(t => t.CreatedByUserID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Ticket>()
                .HasOne(t => t.AssignedToUser)
                .WithMany()
                .HasForeignKey(t => t.AssignedToUserID)
                .OnDelete(DeleteBehavior.Cascade); // keep one as cascade

            builder.Entity<Ticket>()
                .HasOne(t => t.TicketStatus)
                .WithMany()
                .HasForeignKey(t => t.TicketStatusID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Ticket>()
                .HasOne(t => t.TicketPriority)
                .WithMany()
                .HasForeignKey(t => t.TicketPriorityID)
                .OnDelete(DeleteBehavior.Cascade);
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
