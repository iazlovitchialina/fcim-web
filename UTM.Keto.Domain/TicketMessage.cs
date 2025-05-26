using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UTM.Keto.Domain
{
    [Table("TicketMessages")]
    public class TicketMessage : BaseEntity
    {
        [Required]
        public Guid TicketId { get; set; }
        
        [ForeignKey("TicketId")]
        public virtual SupportTicket Ticket { get; set; }
        
        [Required]
        public Guid UserId { get; set; }
        
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        
        [Required]
        public string Content { get; set; }
        
        [Required]
        public DateTime DateSent { get; set; }
        
        [Required]
        public bool IsFromAdmin { get; set; }
        
        public TicketMessage()
        {
            DateSent = DateTime.Now;
        }
    }
} 