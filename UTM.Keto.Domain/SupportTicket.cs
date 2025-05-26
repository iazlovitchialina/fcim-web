using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UTM.Keto.Domain
{
    [Table("SupportTickets")]
    public class SupportTicket : BaseEntity
    {
        [Required]
        public Guid UserId { get; set; }
        
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Subject { get; set; }
        
        [Required]
        public string InitialMessage { get; set; }
        
        [Required]
        public DateTime CreatedDate { get; set; }
        
        public DateTime? ClosedDate { get; set; }
        
        [Required]
        public TicketStatus Status { get; set; }
        
        public string TicketNumber { get; set; }
        
        // Приоритет тикета
        public string Priority { get; set; }
        
        public virtual ICollection<TicketMessage> Messages { get; set; }
        
        public SupportTicket()
        {
            Messages = new List<TicketMessage>();
            CreatedDate = DateTime.Now;
            Status = TicketStatus.Open;
            TicketNumber = GenerateTicketNumber();
            Priority = "Normal";
        }
        
        private string GenerateTicketNumber()
        {
            return "TKT-" + DateTime.Now.ToString("yyyyMMdd") + "-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
        }
    }
    
    public enum TicketStatus
    {
        Open,
        InProgress,
        Closed
    }
} 