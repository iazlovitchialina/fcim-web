using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UTM.Keto.Domain
{
    [Table("Products")]
    public class Product : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
        
        [StringLength(500)]
        public string Description { get; set; }
        
        [StringLength(255)]
        public string ImagePath { get; set; }
        
        public bool InStock { get; set; }
        
        public bool IsFeatured { get; set; }
    }
} 