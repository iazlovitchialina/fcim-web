using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UTM.Keto.Domain
{
    [Table("CartItems")]
    public class CartItem : BaseEntity
    {
        [Required]
        public Guid UserId { get; set; }
        
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        
        [Required]
        public Guid ProductId { get; set; }
        
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
        
        [Required]
        public int Quantity { get; set; }
        
        public DateTime DateAdded { get; set; }
        
        public CartItem()
        {
            DateAdded = DateTime.Now;
        }
    }
} 