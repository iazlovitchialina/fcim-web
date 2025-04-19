using System;
using System.Collections.Generic;

namespace UTM.Keto.Web.Models
{
    public class DailySalesViewModel
    {
        public DateTime Date { get; set; }
        public int OrderCount { get; set; }
        public decimal Revenue { get; set; }
    }
    
    public class UserActivityViewModel
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime LastOrderDate { get; set; }
    }
    
    public class ProductSalesViewModel
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
    }
    
    public class DashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalOrders { get; set; }
        public int TotalProducts { get; set; }
        public decimal TotalRevenue { get; set; }
        public int ActiveUsers { get; set; }
        public List<PopularProductViewModel> PopularProducts { get; set; }
        public List<DailySalesViewModel> DailyOrders { get; set; }
    }
    
    public class PopularProductViewModel
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int QuantitySold { get; set; }
    }
} 