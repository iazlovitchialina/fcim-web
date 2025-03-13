using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UTM.Keto.Domain
{
    [Table("Bookings")]
    public class Booking : BaseEntity
    {
        [Required]
        public Guid UserId { get; set; }
        
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    
        [Required]
        public Guid RoomId { get; set; }
        
        [ForeignKey("RoomId")]
        public virtual Room Room { get; set; }
    
        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CheckInDate { get; set; }
        
        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CheckOutDate { get; set; }
        
        [Required]
        public BookingStatus Status { get; set; }
        
        public decimal TotalPrice { get; set; }
        
        public virtual ICollection<Payment> Payments { get; set; }
        
        public Booking()
        {
            Payments = new List<Payment>();
        }
    }

    public enum BookingStatus
    {
        Pending,
        Confirmed,
        Canceled
    }
}