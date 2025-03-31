using SupportTicketingSystem.Data;

namespace SupportTicketingSystem.Models
{
    public class Ticket
    {

        public int TicketID { get; set; }  // PK
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ResolvedDate { get; set; }

        // FK
        public string CreatedByUserID { get; set; }
        public ApplicationUser CreatedByUser { get; set; }

        public string AssignedToUserID { get; set; }
        public ApplicationUser AssignedToUser { get; set; }

        public int TicketStatusID { get; set; }
        public TicketStatus TicketStatus { get; set; }

        public int TicketPriorityID { get; set; }
        public TicketPriority TicketPriority { get; set; }
        public int TeamId { get; set; }

        // Navigation Properties
        public ICollection<TicketComment> TicketComments { get; set; }
        public ICollection<Attachment> Attachments { get; set; }
        public virtual Team Team { get; set; }
    }
}

