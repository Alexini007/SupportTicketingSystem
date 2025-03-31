namespace SupportTicketingSystem.Models
{
    public class Attachment
    {
        public int AttachmentID { get; set; }  // Primary Key
        public string FileName { get; set; }  // Name of the attached file
        public string FilePath { get; set; }  // Path where the file is stored

        // Foreign Key
        public int TicketID { get; set; }
        public Ticket Ticket { get; set; }
    }
}
