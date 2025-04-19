using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UTM.Keto.Domain;

namespace UTM.Keto.Web.Models
{
    public class OrderViewModel
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; }
        public string UserName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public OrderStatus CurrentStatus { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingMethod { get; set; }
        public List<OrderItemViewModel> Items { get; set; }
        
        public OrderViewModel()
        {
            Items = new List<OrderItemViewModel>();
        }
    }
    
    public class OrderItemViewModel
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; }
        public string ProductImagePath { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
    }
    
    public class OrderStatusViewModel
    {
        public Guid OrderId { get; set; }
        public OrderStatus CurrentStatus { get; set; }
        public OrderStatus NewStatus { get; set; }
        public List<OrderStatus> AvailableStatuses { get; set; }
        
        public OrderStatusViewModel()
        {
            AvailableStatuses = new List<OrderStatus>();
        }
    }
} 