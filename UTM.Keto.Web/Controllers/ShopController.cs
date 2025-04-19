using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using UTM.Keto.Domain;
using UTM.Keto.Infrastructure;
using UTM.Keto.Web.Models;

namespace UTM.Keto.Web.Controllers
{
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShopController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }

        // GET: Shop
        public ActionResult Index()
        {
            var products = _context.Products.ToList();
            return View(products);
        }

        // GET: Shop/Details/5
        public ActionResult Details(Guid id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Shop/Cart
        [Authorize]
        public ActionResult Cart()
        {
            var userId = GetCurrentUserId();
            var model = GetCartViewModel(userId);
            return View(model);
        }

        // POST: Shop/AddToCart
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AddToCart(Guid productId, int quantity = 1)
        {
            var userId = GetCurrentUserId();
            var product = _context.Products.Find(productId);
            
            if (product == null)
            {
                return HttpNotFound();
            }
            
            if (!product.InStock)
            {
                TempData["ErrorMessage"] = "This product is out of stock.";
                return RedirectToAction("Details", new { id = productId });
            }
            
            var cartItem = _context.CartItems.FirstOrDefault(ci => ci.UserId == userId && ci.ProductId == productId);
            
            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                cartItem = new CartItem
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ProductId = productId,
                    Quantity = quantity
                };
                _context.CartItems.Add(cartItem);
            }
            
            _context.SaveChanges();
            
            TempData["SuccessMessage"] = $"Added {quantity} {product.Name} to your cart.";
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
                var cartItem = _context.CartItems.Find(item.Id);
                if (cartItem != null && cartItem.UserId == userId)
                {
                    if (item.Quantity <= 0)
                    {
                        _context.CartItems.Remove(cartItem);
                    }
                    else
                    {
                        cartItem.Quantity = item.Quantity;
                    }
                }
            }
            
            _context.SaveChanges();
            
            TempData["SuccessMessage"] = "Cart updated successfully.";
            return RedirectToAction("Cart");
        }

        // POST: Shop/RemoveFromCart
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveFromCart(Guid cartItemId)
        {
            var userId = GetCurrentUserId();
            var cartItem = _context.CartItems.Find(cartItemId);
            
            if (cartItem != null && cartItem.UserId == userId)
            {
                _context.CartItems.Remove(cartItem);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Item removed from cart.";
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
            
            var model = new CheckoutViewModel
            {
                Cart = cartViewModel
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
                var cartItems = _context.CartItems
                    .Where(ci => ci.UserId == userId)
                    .Include("Product")
                    .ToList();
                
                if (!cartItems.Any())
                {
                    TempData["ErrorMessage"] = "Your cart is empty. Please add items before proceeding to checkout.";
                    return RedirectToAction("Cart");
                }
                
                // Create new order
                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    OrderDate = DateTime.Now,
                    ShippingAddress = model.ShippingAddress,
                    ShippingMethod = model.ShippingMethod,
                    Status = OrderStatus.New,
                    TotalAmount = 0
                };
                
                decimal totalAmount = 0;
                
                // Add order items
                foreach (var cartItem in cartItems)
                {
                    var orderItem = new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = cartItem.Product.Price,
                        Subtotal = cartItem.Product.Price * cartItem.Quantity
                    };
                    
                    totalAmount += orderItem.Subtotal;
                    order.OrderItems.Add(orderItem);
                }
                
                order.TotalAmount = totalAmount;
                
                // Save order
                _context.Orders.Add(order);
                
                // Clear cart
                foreach (var cartItem in cartItems)
                {
                    _context.CartItems.Remove(cartItem);
                }
                
                _context.SaveChanges();
                
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
            var orders = _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
            
            var viewModels = orders.Select(o => new OrderViewModel
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status.ToString(),
                CurrentStatus = o.Status
            }).ToList();
            
            return View(viewModels);
        }

        // GET: Shop/OrderDetails/5
        [Authorize]
        public ActionResult OrderDetails(Guid id)
        {
            var userId = GetCurrentUserId();
            var order = _context.Orders
                .Include("OrderItems.Product")
                .Include("User")
                .FirstOrDefault(o => o.Id == id && o.UserId == userId);
            
            if (order == null)
            {
                return HttpNotFound();
            }
            
            var viewModel = new OrderViewModel
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                UserName = order.User.FullName,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status.ToString(),
                CurrentStatus = order.Status,
                ShippingAddress = order.ShippingAddress,
                ShippingMethod = order.ShippingMethod,
                Items = order.OrderItems.Select(oi => new OrderItemViewModel
                {
                    Id = oi.Id,
                    ProductName = oi.Product.Name,
                    ProductImagePath = oi.Product.ImagePath,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity,
                    Subtotal = oi.Subtotal
                }).ToList()
            };
            
            return View(viewModel);
        }

        private Guid GetCurrentUserId()
        {
            string username = User.Identity.Name;
            var user = _context.Users.FirstOrDefault(u => u.Email == username);
            return user?.Id ?? Guid.Empty;
        }

        private CartViewModel GetCartViewModel(Guid userId)
        {
            var cartItems = _context.CartItems
                .Where(ci => ci.UserId == userId)
                .Include("Product")
                .ToList();
            
            var viewModel = new CartViewModel
            {
                Items = cartItems.Select(ci => new CartItemViewModel
                {
                    Id = ci.Id,
                    ProductId = ci.ProductId,
                    ProductName = ci.Product.Name,
                    ProductImagePath = ci.Product.ImagePath,
                    Price = ci.Product.Price,
                    Quantity = ci.Quantity,
                    Subtotal = ci.Product.Price * ci.Quantity
                }).ToList()
            };
            
            viewModel.TotalAmount = viewModel.Items.Sum(item => item.Subtotal);
            viewModel.ItemCount = viewModel.Items.Sum(item => item.Quantity);
            
            return viewModel;
        }
    }
} 