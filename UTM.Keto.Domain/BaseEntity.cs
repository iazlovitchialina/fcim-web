using System;
using System.ComponentModel.DataAnnotations;

namespace UTM.Keto.Domain
{
    public abstract class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
        
        public BaseEntity()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
        }
    }
} 