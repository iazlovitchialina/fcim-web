using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UTM.Keto.Application;
using UTM.Keto.Application.Interfaces;
using UTM.Keto.Domain;
using UTM.Keto.Domain.DTOs;
using UTM.Keto.Web.Models;

namespace UTM.Keto.Web.Controllers
{
    public class ShopController : Controller
    {
        private readonly IProductBL _productBL;
        private readonly ICartBL _cartBL;
        private readonly IOrderBL _orderBL;
        private readonly IUserBL _userBL;

        public ShopController()
        {
            var factory = BusinessLogicFactory.Instance;
            _productBL = factory.GetProductBL();
            _cartBL = factory.GetCartBL();
            _orderBL = factory.GetOrderBL();
            _userBL = factory.GetUserBL();
        }

        // GET: Shop
        public ActionResult Index()
        {
            var products = _productBL.GetAllProducts();
            
            // Получаем информацию о текущем пользователе, если он авторизован
            if (User.Identity.IsAuthenticated)
            {
                var userId = GetCurrentUserId();
                var user = _userBL.GetUserById(userId);
                ViewBag.UserName = user.FullName;
            }
            
            return View(products);
        }

        // GET: Shop/Details/5
        public ActionResult Details(Guid id)
        {
            var product = _productBL.GetProductById(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            
            // Получаем информацию о пользователе
            if (User.Identity.IsAuthenticated)
            {
                var userId = GetCurrentUserId();
                var user = _userBL.GetUserById(userId);
                ViewBag.UserName = user.FullName;
            }
            
            return View(product);
        }

        // GET: Shop/Cart
        [Authorize]
        public ActionResult Cart()
        {
            var userId = GetCurrentUserId();
            var model = GetCartViewModel(userId);
            
            // Добавляем информацию о пользователе
            var user = _userBL.GetUserById(userId);
            ViewBag.UserName = user.FullName;
            
            return View(model);
        }

        // POST: Shop/AddToCart
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AddToCart(Guid productId, int quantity = 1)
        {
            var userId = GetCurrentUserId();
            var product = _productBL.GetProductById(productId);
            
            if (product == null)
            {
                return HttpNotFound();
            }
            
            if (!product.InStock)
            {
                TempData["ErrorMessage"] = "This product is out of stock.";
                return RedirectToAction("Details", new { id = productId });
            }
            
            var cartAction = new CartActionDto
            {
                UserId = userId.GetHashCode(),
                ProductId = productId.GetHashCode(),
                Quantity = quantity,
                Action = "Add"
            };
            
            var result = _cartBL.AddToCart(cartAction);
            
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = $"Added {quantity} {product.Name} to your cart.";
            }
            else
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
            }
            
            return RedirectToAction("Cart");
        }

        // POST: Shop/UpdateCart
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateCart(List<CartItemViewModel> items)
        {
            var userId = GetCurrentUserId();
            
            foreach (var item in items)
            {
                var cartAction = new CartActionDto
                {
                    UserId = userId.GetHashCode(),
                    ProductId = item.ProductId.GetHashCode(),
                    Quantity = item.Quantity,
                    Action = "Update"
                };
                
                if (item.Quantity <= 0)
                {
                    _cartBL.RemoveFromCart(cartAction);
                }
                else
                {
                    _cartBL.UpdateCartItemQuantity(cartAction);
                }
            }
            
            TempData["SuccessMessage"] = "Cart updated successfully.";
            return RedirectToAction("Cart");
        }

        // POST: Shop/RemoveFromCart
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveFromCart(Guid cartItemId, Guid productId)
        {
            var userId = GetCurrentUserId();
            
            var cartAction = new CartActionDto
            {
                UserId = userId.GetHashCode(),
                ProductId = productId.GetHashCode(),
                Quantity = 0,
                Action = "Remove"
            };
            
            var result = _cartBL.RemoveFromCart(cartAction);
            
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Item removed from cart.";
            }
            else
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
            }
            
            return RedirectToAction("Cart");
        }

        // GET: Shop/Checkout
        [Authorize]
        public ActionResult Checkout()
        {
            var userId = GetCurrentUserId();
            var cartViewModel = GetCartViewModel(userId);
            
            if (cartViewModel.ItemCount == 0)
            {
                TempData["ErrorMessage"] = "Your cart is empty. Please add items before proceeding to checkout.";
                return RedirectToAction("Cart");
            }
            
            // Получаем данные пользователя для оформления заказа
            var user = _userBL.GetUserById(userId);
            
            var model = new CheckoutViewModel
            {
                Cart = cartViewModel,
                UserName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };
            
            return View(model);
        }

        // POST: Shop/Checkout
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(CheckoutViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = GetCurrentUserId();
                var cart = _cartBL.GetCart(userId.GetHashCode());
                
                if (cart.Items.Count == 0)
                {
                    TempData["ErrorMessage"] = "Your cart is empty. Please add items before proceeding to checkout.";
                    return RedirectToAction("Cart");
                }
                
                var order = _orderBL.CreateOrder(userId.GetHashCode(), model.ShippingAddress);
                
                if (order == null)
                {
                    TempData["ErrorMessage"] = "There was a problem creating your order. Please try again.";
                    return RedirectToAction("Checkout");
                }
                
                TempData["SuccessMessage"] = "Order placed successfully. Order number: " + order.OrderNumber;
                return RedirectToAction("MyOrders");
            }
            
            // If we got this far, something failed, redisplay form
            var userId2 = GetCurrentUserId();
            model.Cart = GetCartViewModel(userId2);
            return View(model);
        }

        // GET: Shop/MyOrders
        [Authorize]
        public ActionResult MyOrders()
        {
            var userId = GetCurrentUserId();
            var orders = _orderBL.GetOrdersByUserId(userId);
            var user = _userBL.GetUserById(userId);
            
            var orderViewModels = orders.Select(o => new OrderViewModel
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                UserName = user.FullName,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status.ToString(),
                CurrentStatus = o.Status.ToString()
            }).ToList();
            
            return View(orderViewModels);
        }

        // GET: Shop/OrderDetails/5
        [Authorize]
        public ActionResult OrderDetails(Guid id)
        {
            var userId = GetCurrentUserId();
            var order = _orderBL.GetOrderById(id);
            
            if (order == null)
            {
                return HttpNotFound();
            }
            
            if (order.UserId != userId)
            {
                return new HttpUnauthorizedResult();
            }
            
            var user = _userBL.GetUserById(userId);
            
            var orderViewModel = new OrderViewModel
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                UserName = user.FullName,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status.ToString(),
                CurrentStatus = order.Status.ToString(),
                ShippingAddress = order.ShippingAddress,
                ShippingMethod = order.ShippingMethod,
                Items = order.OrderItems.Select(oi => new OrderItemViewModel
                {
                    Id = oi.Id,
                    ProductName = _productBL.GetProductById(oi.ProductId).Name,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity,
                    Subtotal = oi.UnitPrice * oi.Quantity
                }).ToList()
            };
            
            return View(orderViewModel);
        }

        private Guid GetCurrentUserId()
        {
            // Получение идентификатора пользователя из билета аутентификации
            var ticket = System.Web.Security.FormsAuthentication.Decrypt(Request.Cookies[System.Web.Security.FormsAuthentication.FormsCookieName].Value);
            var userData = ticket.UserData.Split('|');
            return new Guid(userData[0]);
        }

        private CartViewModel GetCartViewModel(Guid userId)
        {
            var cart = _cartBL.GetCart(userId.GetHashCode());
            
            var model = new CartViewModel
            {
                Items = cart.Items.Select(i => new CartItemViewModel
                {
                    Id = new Guid(i.Id.ToString()),
                    ProductId = new Guid(i.ProductId.ToString()),
                    ProductName = i.ProductName,
                    Price = i.Price,
                    Quantity = i.Quantity,
                    Subtotal = i.SubTotal
                }).ToList(),
                TotalAmount = cart.TotalPrice,
                ItemCount = cart.Items.Count
            };
            
            return model;
        }
    }
} 