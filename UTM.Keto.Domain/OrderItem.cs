using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UTM.Keto.Domain
{
    [Table("OrderItems")]
    public class OrderItem : BaseEntity
    {
        [Required]
        public Guid OrderId { get; set; }
        
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }
        
        [Required]
        public Guid ProductId { get; set; }
        
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
        
        [Required]
        public int Quantity { get; set; }
        
        [Required]
        [DataType(DataType.Currency)]
        public decimal UnitPrice { get; set; }
        
        [Required]
        [DataType(DataType.Currency)]
        public decimal Subtotal { get; set; }
    }
} 