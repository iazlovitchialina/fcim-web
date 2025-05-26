using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using UTM.Keto.Application;
using UTM.Keto.Application.Interfaces;
using UTM.Keto.Domain;
using UTM.Keto.Web.Filters;
using UTM.Keto.Web.Models;

namespace UTM.Keto.Web.Controllers
{
    [CustomAuthorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUserBL _userBL;
        private readonly IOrderBL _orderBL;
        private readonly IProductBL _productBL;

        public AdminController()
        {
            var factory = BusinessLogicFactory.Instance;
            _userBL = factory.GetUserBL();
            _orderBL = factory.GetOrderBL();
            _productBL = factory.GetProductBL();
        }

        public ActionResult Index()
        {
            // Получаем статистику для дашборда
            var users = _userBL.GetAllUsers();
            var orders = _orderBL.GetAllOrders();
            var products = _productBL.GetAllProducts();
            
            ViewBag.TotalUsers = users.Count;
            ViewBag.TotalOrders = orders.Count;
            ViewBag.TotalProducts = products.Count;
            ViewBag.TotalRevenue = orders.Sum(o => o.TotalAmount);
            
            // Получаем активных пользователей (с заказами за последние 30 дней)
            var thirtyDaysAgo = DateTime.Now.AddDays(-30);
            ViewBag.ActiveUsers = orders
                .Where(o => o.OrderDate >= thirtyDaysAgo)
                .Select(o => o.UserId)
                .Distinct()
                .Count();
                
            // Получаем самые популярные товары (топ-5)
            var orderItems = orders.SelectMany(o => o.OrderItems).ToList();
            ViewBag.PopularProducts = orderItems
                .GroupBy(oi => oi.ProductId)
                .Select(g => new { 
                    ProductId = g.Key, 
                    ProductName = _productBL.GetProductById(g.Key).Name,
                    Count = g.Sum(oi => oi.Quantity) 
                })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToList();
                
            // Получаем заказы по дням за последний месяц
            ViewBag.DailyOrders = orders
                .Where(o => o.OrderDate >= thirtyDaysAgo)
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new { 
                    Date = g.Key, 
                    Count = g.Count(),
                    Revenue = g.Sum(o => o.TotalAmount)
                })
                .OrderBy(x => x.Date)
                .ToList();
                
            return View();
        }
        
        // GET: Admin/Reports
        public ActionResult Reports()
        {
            return View();
        }
        
        // GET: Admin/SalesReport
        public ActionResult SalesReport(DateTime? startDate = null, DateTime? endDate = null)
        {
            startDate = startDate ?? DateTime.Now.AddDays(-30);
            endDate = endDate ?? DateTime.Now;
            
            var orders = _orderBL.GetAllOrders();
            var salesData = orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new DailySalesViewModel
                {
                    Date = g.Key,
                    OrderCount = g.Count(),
                    Revenue = g.Sum(o => o.TotalAmount)
                })
                .OrderBy(x => x.Date)
                .ToList();
            
            ViewBag.StartDate = startDate.Value.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate.Value.ToString("yyyy-MM-dd");
            ViewBag.TotalRevenue = salesData.Sum(x => x.Revenue);
            ViewBag.TotalOrders = salesData.Sum(x => x.OrderCount);
            
            return View(salesData);
        }
        
        // GET: Admin/UsersReport
        public ActionResult UsersReport(DateTime? startDate = null, DateTime? endDate = null)
        {
            startDate = startDate ?? DateTime.Now.AddDays(-30);
            endDate = endDate ?? DateTime.Now;
            
            var orders = _orderBL.GetAllOrders();
            var activeUsers = orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .Select(o => o.UserId)
                .Distinct()
                .ToList();
                
            var userData = new List<UserActivityViewModel>();
            
            foreach (var userId in activeUsers)
            {
                var user = _userBL.GetUserById(userId);
                if (user != null)
                {
                    var userOrders = orders.Where(o => o.UserId == userId && o.OrderDate >= startDate && o.OrderDate <= endDate).ToList();
                    var orderCount = userOrders.Count;
                    var totalSpent = userOrders.Sum(o => o.TotalAmount);
                        
                    userData.Add(new UserActivityViewModel
                    {
                        UserId = userId,
                        UserName = user.FullName,
                        Email = user.Email,
                        OrderCount = orderCount,
                        TotalSpent = totalSpent,
                        LastOrderDate = orders
                            .Where(o => o.UserId == userId)
                            .OrderByDescending(o => o.OrderDate)
                            .Select(o => o.OrderDate)
                            .FirstOrDefault()
                    });
                }
            }
            
            ViewBag.StartDate = startDate.Value.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate.Value.ToString("yyyy-MM-dd");
            ViewBag.TotalActiveUsers = userData.Count;
            
            return View(userData.OrderByDescending(u => u.TotalSpent).ToList());
        }
        
        // GET: Admin/ProductsReport
        public ActionResult ProductsReport(DateTime? startDate = null, DateTime? endDate = null)
        {
            startDate = startDate ?? DateTime.Now.AddDays(-30);
            endDate = endDate ?? DateTime.Now;
            
            var orders = _orderBL.GetAllOrders()
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .ToList();
            
            var orderItems = orders.SelectMany(o => o.OrderItems).ToList();
            
            var productData = orderItems
                .GroupBy(oi => oi.ProductId)
                .Select(g => new ProductSalesViewModel
                {
                    ProductId = g.Key,
                    ProductName = _productBL.GetProductById(g.Key).Name,
                    QuantitySold = g.Sum(oi => oi.Quantity),
                    Revenue = g.Sum(oi => oi.UnitPrice * oi.Quantity)
                })
                .OrderByDescending(x => x.QuantitySold)
                .ToList();
            
            ViewBag.StartDate = startDate.Value.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate.Value.ToString("yyyy-MM-dd");
            ViewBag.TotalRevenue = productData.Sum(x => x.Revenue);
            ViewBag.TotalSold = productData.Sum(x => x.QuantitySold);
            
            return View(productData);
        }

        public ActionResult Users()
        {
            var users = _userBL.GetAllUsers();
            return View(users);
        }

        public ActionResult EditUser(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            var user = _userBL.GetUserById(id.Value);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(User user)
        {
            if (ModelState.IsValid)
            {
                var dbUser = _userBL.GetUserById(user.Id);
                if (dbUser == null)
                {
                    return HttpNotFound();
                }

                dbUser.FullName = user.FullName;
                dbUser.Email = user.Email;
                dbUser.PhoneNumber = user.PhoneNumber;
                dbUser.Role = user.Role;

                _userBL.UpdateUser(dbUser);
                
                TempData["SuccessMessage"] = "User has been updated successfully.";
                return RedirectToAction("Users");
            }
            return View(user);
        }

        public ActionResult DeleteUser(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            var user = _userBL.GetUserById(id.Value);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteUserConfirmed(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            var user = _userBL.GetUserById(id.Value);
            if (user == null)
            {
                return HttpNotFound();
            }

            string userName = user.FullName;
            
            _userBL.DeleteUser(id.Value);
            
            TempData["SuccessMessage"] = $"User '{userName}' has been deleted successfully.";
            return RedirectToAction("Users");
        }
    }
} 