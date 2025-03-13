using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UTM.Keto.Domain
{
    [Table("Hotels")]
    public class Hotel : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Address { get; set; }
        
        [Required]
        [StringLength(50)]
        public string City { get; set; }
        
        public virtual ICollection<Room> Rooms { get; set; }
        
        public Hotel()
        {
            Rooms = new List<Room>();
        }
    }
}