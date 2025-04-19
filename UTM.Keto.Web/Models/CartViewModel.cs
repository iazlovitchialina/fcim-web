using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UTM.Keto.Domain;

namespace UTM.Keto.Web.Models
{
    public class CartViewModel
    {
        public List<CartItemViewModel> Items { get; set; }
        public decimal TotalAmount { get; set; }
        public int ItemCount { get; set; }
        
        public CartViewModel()
        {
            Items = new List<CartItemViewModel>();
        }
    }
    
    public class CartItemViewModel
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImagePath { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
    }
} 