using System;
using System.Collections.Generic;
using System.Linq;
using UTM.Keto.Application.Interfaces;
using UTM.Keto.Domain;
using UTM.Keto.Infrastructure;

namespace UTM.Keto.Application.BLogic
{
    public class OrderBL : IOrderBL
    {
        private readonly ApplicationDbContext _db;
        private readonly ICartBL _cartBL;

        public OrderBL()
        {
            _db = new ApplicationDbContext();
            _cartBL = BusinessLogicFactory.Instance.GetCartBL();
        }

        public Order CreateOrder(int userId, string shippingAddress)
        {
            var cart = _cartBL.GetCart(userId);
            if (cart.Items.Count == 0)
            {
                return null;
            }

            var order = new Order
            {
                UserId = new Guid(userId.ToString()),
                OrderDate = DateTime.Now,
                ShippingAddress = shippingAddress,
                ShippingMethod = "Standard", // Значение по умолчанию
                Status = OrderStatus.New,
                TotalAmount = cart.TotalPrice
            };

            _db.Orders.Add(order);
            _db.SaveChanges();

            foreach (var item in cart.Items)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = new Guid(item.ProductId.ToString()),
                    Quantity = item.Quantity,
                    UnitPrice = item.Price
                };
                _db.OrderItems.Add(orderItem);
            }

            _db.SaveChanges();
            _cartBL.ClearCart(userId);

            return order;
        }

        public Order CreateOrder(Guid userId, string shippingAddress)
        {
            return CreateOrder(userId.GetHashCode(), shippingAddress);
        }

        public Order GetOrderById(int orderId)
        {
            return _db.Orders.AsQueryable().FirstOrDefault(o => o.Id.GetHashCode() == orderId);
        }

        public Order GetOrderById(Guid orderId)
        {
            return _db.Orders.AsQueryable().FirstOrDefault(o => o.Id == orderId);
        }

        public List<Order> GetOrdersByUserId(int userId)
        {
            return _db.Orders.AsQueryable().Where(o => o.UserId.GetHashCode() == userId).ToList();
        }

        public List<Order> GetOrdersByUserId(Guid userId)
        {
            return _db.Orders.AsQueryable().Where(o => o.UserId == userId).ToList();
        }

        public List<Order> GetAllOrders()
        {
            return _db.Orders.ToList();
        }

        public void UpdateOrderStatus(int orderId, string status)
        {
            var order = _db.Orders.AsQueryable().FirstOrDefault(o => o.Id.GetHashCode() == orderId);
            if (order != null)
            {
                if (Enum.TryParse(status, out OrderStatus orderStatus))
                {
                    order.Status = orderStatus;
                    _db.SaveChanges();
                }
            }
        }

        public void UpdateOrderStatus(Guid orderId, string status)
        {
            var order = _db.Orders.AsQueryable().FirstOrDefault(o => o.Id == orderId);
            if (order != null)
            {
                if (Enum.TryParse(status, out OrderStatus orderStatus))
                {
                    order.Status = orderStatus;
                    _db.SaveChanges();
                }
            }
        }
    }
} 