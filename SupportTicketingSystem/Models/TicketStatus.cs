namespace SupportTicketingSystem.Models
{
    public class TicketStatus
    {
        public int TicketStatusID { get; set; } // Primary Key
        public string Name { get; set; } // e.g., "Open", "In Progress", "Closed"
    }
}
