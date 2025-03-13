using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UTM.Keto.Domain
{
    [Table("Rooms")]
    public class Room : BaseEntity
    {
        [Required]
        [Range(1, 9999)]
        public int Number { get; set; }
        
        [Required]
        public RoomType Type { get; set; }
        
        [Required]
        public decimal PricePerNight { get; set; }
        
        public bool IsAvailable { get; set; }
        
        [Required]
        public Guid HotelId { get; set; }
        
        [ForeignKey("HotelId")]
        public virtual Hotel Hotel { get; set; }
        
        public virtual ICollection<Booking> Bookings { get; set; }
        
        public Room()
        {
            Bookings = new List<Booking>();
        }
    }

    public enum RoomType
    {
        Single,
        Double,
        Suite
    }
}