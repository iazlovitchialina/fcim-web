using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UTM.Keto.Domain;
using UTM.Keto.Infrastructure;
using UTM.Keto.Web.Filters;
using UTM.Keto.Web.Models;

namespace UTM.Keto.Web.Controllers
{
    [CustomAuthorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController()
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

        // GET: Order
        public ActionResult Index()
        {
            var orders = _context.Orders
                .Include("User")
                .OrderByDescending(o => o.OrderDate)
                .ToList();
            
            var viewModels = orders.Select(o => new OrderViewModel
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                UserName = o.User.FullName,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status.ToString(),
                CurrentStatus = o.Status
            }).ToList();
            
            return View(viewModels);
        }

        // GET: Order/Details/5
        public ActionResult Details(Guid id)
        {
            var order = _context.Orders
                .Include("OrderItems.Product")
                .Include("User")
                .FirstOrDefault(o => o.Id == id);
            
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

        // GET: Order/UpdateStatus/5
        public ActionResult UpdateStatus(Guid id)
        {
            var order = _context.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            
            var model = new OrderStatusViewModel
            {
                OrderId = order.Id,
                CurrentStatus = order.Status,
                NewStatus = order.Status,
                AvailableStatuses = GetAvailableStatuses(order.Status)
            };
            
            return View(model);
        }

        // POST: Order/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateStatus(OrderStatusViewModel model)
        {
            if (ModelState.IsValid)
            {
                var order = _context.Orders.Find(model.OrderId);
                if (order == null)
                {
                    return HttpNotFound();
                }
                
                // Validate status change
                if (!IsValidStatusTransition(order.Status, model.NewStatus))
                {
                    ModelState.AddModelError("", "Invalid status transition. You cannot skip status steps.");
                    model.AvailableStatuses = GetAvailableStatuses(order.Status);
                    return View(model);
                }
                
                order.Status = model.NewStatus;
                _context.SaveChanges();
                
                TempData["SuccessMessage"] = $"Order status updated to {model.NewStatus}.";
                return RedirectToAction("Details", new { id = model.OrderId });
            }
            
            // If we got this far, something failed, redisplay form
            var order2 = _context.Orders.Find(model.OrderId);
            if (order2 == null)
            {
                return HttpNotFound();
            }
            
            model.AvailableStatuses = GetAvailableStatuses(order2.Status);
            return View(model);
        }

        private bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            // Can't go backwards
            if (newStatus < currentStatus)
            {
                return false;
            }
            
            // Can't skip steps
            return (int)newStatus <= (int)currentStatus + 1;
        }

        private List<OrderStatus> GetAvailableStatuses(OrderStatus currentStatus)
        {
            var statuses = new List<OrderStatus> { currentStatus };
            
            // Add next status if not at the end
            if (currentStatus != OrderStatus.Delivered)
            {
                statuses.Add(currentStatus + 1);
            }
            
            return statuses;
        }
    }
} 