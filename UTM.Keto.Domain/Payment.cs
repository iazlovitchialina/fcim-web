using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UTM.Keto.Domain
{
    [Table("Payments")]
    public class Payment : BaseEntity
    {
        [Required]
        public Guid BookingId { get; set; }
        
        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; }
        
        [Required]
        public decimal Amount { get; set; }
        
        [Required]
        public PaymentStatus Status { get; set; }
        
        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime PaymentDate { get; set; }
        
        [StringLength(100)]
        public string TransactionId { get; set; }
        
        [StringLength(500)]
        public string Notes { get; set; }
        
        [StringLength(50)]
        public string PaymentMethod { get; set; }
    }

    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed,
        Refunded
    }
}