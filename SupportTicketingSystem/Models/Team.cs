namespace SupportTicketingSystem.Models
{
    public class Team
    {
        public int TeamId { get; set; }  // Primary key for the Team table
        public string TeamName { get; set; }  // Name of the team (e.g., Development, Support, Sales)

        // Navigation property to reference tickets assigned to the team
        public ICollection<Ticket> Tickets { get; set; }
    }
}
