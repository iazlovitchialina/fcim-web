using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UTM.Keto.Domain
{
    [Table("Orders")]
    public class Order : BaseEntity
    {
        [Required]
        public Guid UserId { get; set; }
        
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        
        [Required]
        public DateTime OrderDate { get; set; }
        
        [Required]
        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }
        
        [Required]
        [StringLength(100)]
        public string ShippingAddress { get; set; }
        
        [Required]
        [StringLength(50)]
        public string ShippingMethod { get; set; }
        
        [Required]
        public OrderStatus Status { get; set; }
        
        public string OrderNumber { get; set; }
        
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        
        public Order()
        {
            OrderItems = new List<OrderItem>();
            OrderDate = DateTime.Now;
            Status = OrderStatus.New;
            OrderNumber = GenerateOrderNumber();
        }
        
        private string GenerateOrderNumber()
        {
            return "ORD-" + DateTime.Now.ToString("yyyyMMdd") + "-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
        }
    }
    
    public enum OrderStatus
    {
        New,
        Processing,
        Shipped,
        Delivered
    }
} 