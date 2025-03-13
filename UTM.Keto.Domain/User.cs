using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UTM.Keto.Domain;

namespace UTM.Keto.Domain
{
    [Table("Users")]
    public class User : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }
        
        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }
        
        [StringLength(20)]
        public string PhoneNumber { get; set; }
        
        [Required]
        public UserRole Role { get; set; }
        
        public virtual ICollection<Booking> Bookings { get; set; }
        
        public User()
        {
            Bookings = new List<Booking>();
        }
    }

    public enum UserRole
    {
        Guest,
        Admin
    }
}