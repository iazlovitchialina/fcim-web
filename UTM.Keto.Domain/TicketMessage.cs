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
        public Guid SenderId { get; set; }
        
        [ForeignKey("SenderId")]
        public virtual User Sender { get; set; }
        
        [Required]
        public string Message { get; set; }
        
        [Required]
        public DateTime SentDate { get; set; }
        
        [Required]
        public bool IsFromAdmin { get; set; }
        
        public TicketMessage()
        {
            SentDate = DateTime.Now;
        }
    }
} 