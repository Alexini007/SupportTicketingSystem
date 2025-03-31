using SupportTicketingSystem.Data;

namespace SupportTicketingSystem.Models
{
    public class TicketComment
    {
        public int TicketCommentID { get; set; }  // Primary Key
        public string CommentText { get; set; }  // Comment text
        public DateTime CreatedDate { get; set; }

        // Foreign Keys
        public int TicketID { get; set; }
        public Ticket Ticket { get; set; }

        public string UserID { get; set; }
        public ApplicationUser User { get; set; }
    }
}
