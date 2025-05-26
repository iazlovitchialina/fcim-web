using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UTM.Keto.Application;
using UTM.Keto.Application.Interfaces;
using UTM.Keto.Domain;
using UTM.Keto.Web.Filters;
using UTM.Keto.Web.Models;

namespace UTM.Keto.Web.Controllers
{
    [CustomAuthorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly IOrderBL _orderBL;
        private readonly ICartBL _cartBL;

        public OrderController()
        {
            var factory = BusinessLogicFactory.Instance;
            _orderBL = factory.GetOrderBL();
            _cartBL = factory.GetCartBL();
        }

        public ActionResult Index()
        {
            int userId = GetCurrentUserId();
            var orders = _orderBL.GetOrdersByUserId(userId);
            
            // Преобразуем Order в OrderViewModel
            var orderViewModels = orders.Select(o => new OrderViewModel
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                UserName = o.User?.FullName ?? "Неизвестно",
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status.ToString(),
                CurrentStatus = o.Status.ToString(),
                ShippingAddress = o.ShippingAddress,
                ShippingMethod = o.ShippingMethod
            }).ToList();
            
            return View(orderViewModels);
        }

        public ActionResult Details(int id)
        {
            var order = _orderBL.GetOrderById(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            // Преобразуем Order в OrderViewModel
            var orderViewModel = new OrderViewModel
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                UserName = order.User?.FullName ?? "Неизвестно",
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status.ToString(),
                CurrentStatus = order.Status.ToString(),
                ShippingAddress = order.ShippingAddress,
                ShippingMethod = order.ShippingMethod,
                Items = order.OrderItems?.Select(item => new OrderItemViewModel
                {
                    Id = item.Id,
                    ProductName = item.Product?.Name ?? "Неизвестно",
                    ProductImagePath = item.Product?.ImagePath,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    Subtotal = item.UnitPrice * item.Quantity
                }).ToList() ?? new List<OrderItemViewModel>()
            };

            // Проверка, что заказ принадлежит текущему пользователю или пользователь администратор
            if (order.UserId.GetHashCode() != GetCurrentUserId() && !User.IsInRole("Admin"))
            {
                return new HttpUnauthorizedResult();
            }

            return View(orderViewModel);
        }

        public ActionResult Checkout()
        {
            int userId = GetCurrentUserId();
            var cart = _cartBL.GetCart(userId);
            
            if (cart.Items.Count == 0)
            {
                return RedirectToAction("Index", "Shop");
            }
            
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PlaceOrder(string shippingAddress)
        {
            int userId = GetCurrentUserId();
            
            var order = _orderBL.CreateOrder(userId, shippingAddress);
            if (order == null)
            {
                return RedirectToAction("Index", "Shop");
            }
            
            return RedirectToAction("OrderComplete", new { id = order.Id.GetHashCode() });
        }

        public ActionResult OrderComplete(int id)
        {
            var order = _orderBL.GetOrderById(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            
            // Преобразуем Order в OrderViewModel
            var orderViewModel = new OrderViewModel
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                UserName = order.User?.FullName ?? "Неизвестно",
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status.ToString(),
                CurrentStatus = order.Status.ToString(),
                ShippingAddress = order.ShippingAddress,
                ShippingMethod = order.ShippingMethod
            };
            
            // Проверка, что заказ принадлежит текущему пользователю
            if (order.UserId.GetHashCode() != GetCurrentUserId() && !User.IsInRole("Admin"))
            {
                return new HttpUnauthorizedResult();
            }
            
            return View(orderViewModel);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult UpdateStatus(int id, string status)
        {
            _orderBL.UpdateOrderStatus(id, status);
            return RedirectToAction("Details", new { id });
        }

        private int GetCurrentUserId()
        {
            var identity = User.Identity as System.Security.Claims.ClaimsIdentity;
            if (identity != null)
            {
                var userIdClaim = identity.Claims.FirstOrDefault(c => c.Type == "UserId");
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    return userId;
                }
            }
            
            // Альтернативный способ получения ID из форм-аутентификации
            var ticket = System.Web.Security.FormsAuthentication.Decrypt(Request.Cookies[System.Web.Security.FormsAuthentication.FormsCookieName].Value);
            var userData = ticket.UserData.Split('|');
            return int.Parse(userData[0]);
        }
    }
} 