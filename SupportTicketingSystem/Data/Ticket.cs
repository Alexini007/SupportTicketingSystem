﻿using System.ComponentModel.DataAnnotations;

namespace SupportTicketingSystem.Data
{
    public class Ticket
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 5)]
        public string Subject { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Team { get; set; }

        [Required]
        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }

        // Foreign key to ApplicationUser
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}