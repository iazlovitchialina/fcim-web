using System;
using System.Collections.Generic;
using UTM.Keto.Domain;

namespace UTM.Keto.Application.Interfaces
{
    public interface IOrderBL
    {
        Order CreateOrder(int userId, string shippingAddress);
        Order CreateOrder(Guid userId, string shippingAddress);
        Order GetOrderById(int orderId);
        Order GetOrderById(Guid orderId);
        List<Order> GetOrdersByUserId(int userId);
        List<Order> GetOrdersByUserId(Guid userId);
        List<Order> GetAllOrders();
        void UpdateOrderStatus(int orderId, string status);
        void UpdateOrderStatus(Guid orderId, string status);
    }
} 