using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using UTM.Keto.Domain;
using UTM.Keto.Infrastructure;
using UTM.Keto.Web.Filters;
using UTM.Keto.Web.Models;

namespace UTM.Keto.Web.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewController()
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

        // GET: Review
        public ActionResult Index()
        {
            var approvedReviews = _context.Reviews
                .Where(r => r.Status == ReviewStatus.Approved)
                .OrderByDescending(r => r.CreatedDate)
                .Include(r => r.User)
                .Include(r => r.Product)
                .ToList();
                
            var viewModels = approvedReviews.Select(r => new ReviewViewModel
            {
                Id = r.Id,
                UserName = r.User.FullName,
                Title = r.Title,
                Content = r.Content,
                Rating = r.Rating,
                CreatedDate = r.CreatedDate,
                Status = r.Status.ToString(),
                CurrentStatus = r.Status,
                ProductName = r.Product?.Name,
                ProductId = r.ProductId
            }).ToList();
            
            return View(viewModels);
        }
        
        // GET: Review/Create
        [Authorize]
        public ActionResult Create(Guid? productId = null)
        {
            var model = new CreateReviewViewModel();
            
            if (productId.HasValue)
            {
                var product = _context.Products.Find(productId.Value);
                if (product != null)
                {
                    model.ProductId = product.Id;
                    ViewBag.ProductName = product.Name;
                }
            }
            
            return View(model);
        }
        
        // POST: Review/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateReviewViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = GetCurrentUserId();
                
                var review = new Review
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Title = model.Title,
                    Content = model.Content,
                    Rating = model.Rating,
                    CreatedDate = DateTime.Now,
                    Status = ReviewStatus.PendingModeration,
                    ProductId = model.ProductId
                };
                
                _context.Reviews.Add(review);
                _context.SaveChanges();
                
                TempData["SuccessMessage"] = "Ваш отзыв успешно отправлен и будет опубликован после проверки модератором.";
                
                if (model.ProductId.HasValue)
                {
                    return RedirectToAction("Details", "Shop", new { id = model.ProductId });
                }
                
                return RedirectToAction("Index");
            }
            
            return View(model);
        }
        
        // GET: Review/Details/5
        public ActionResult Details(Guid id)
        {
            var review = _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Product)
                .FirstOrDefault(r => r.Id == id);
                
            if (review == null)
            {
                return HttpNotFound();
            }
            
            var viewModel = new ReviewViewModel
            {
                Id = review.Id,
                UserName = review.User.FullName,
                Title = review.Title,
                Content = review.Content,
                Rating = review.Rating,
                CreatedDate = review.CreatedDate,
                Status = review.Status.ToString(),
                CurrentStatus = review.Status,
                ModerationComment = review.ModerationComment,
                ProductName = review.Product?.Name,
                ProductId = review.ProductId
            };
            
            return View(viewModel);
        }
        
        // GET: Review/Moderation
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Moderation()
        {
            var pendingReviews = _context.Reviews
                .Where(r => r.Status == ReviewStatus.PendingModeration)
                .OrderByDescending(r => r.CreatedDate)
                .Include(r => r.User)
                .Include(r => r.Product)
                .ToList();
                
            var viewModels = pendingReviews.Select(r => new ReviewViewModel
            {
                Id = r.Id,
                UserName = r.User.FullName,
                Title = r.Title,
                Content = r.Content,
                Rating = r.Rating,
                CreatedDate = r.CreatedDate,
                Status = r.Status.ToString(),
                CurrentStatus = r.Status,
                ProductName = r.Product?.Name,
                ProductId = r.ProductId
            }).ToList();
            
            return View(viewModels);
        }
        
        // GET: Review/Moderate/5
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Moderate(Guid id)
        {
            var review = _context.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            
            var model = new ModerateReviewViewModel
            {
                ReviewId = review.Id,
                ModerationComment = review.ModerationComment,
                NewStatus = review.Status
            };
            
            return View(model);
        }
        
        // POST: Review/Moderate
        [HttpPost]
        [CustomAuthorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Moderate(ModerateReviewViewModel model)
        {
            if (ModelState.IsValid)
            {
                var review = _context.Reviews.Find(model.ReviewId);
                if (review == null)
                {
                    return HttpNotFound();
                }
                
                review.Status = model.NewStatus;
                review.ModerationComment = model.ModerationComment;
                
                _context.SaveChanges();
                
                TempData["SuccessMessage"] = $"Статус отзыва изменен на {model.NewStatus}.";
                return RedirectToAction("Moderation");
            }
            
            return View(model);
        }
        
        // POST: Review/Approve/5
        [HttpPost]
        [CustomAuthorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(Guid id)
        {
            var review = _context.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            
            review.Status = ReviewStatus.Approved;
            _context.SaveChanges();
            
            TempData["SuccessMessage"] = "Отзыв одобрен и опубликован.";
            return RedirectToAction("Moderation");
        }
        
        // POST: Review/Reject/5
        [HttpPost]
        [CustomAuthorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Reject(Guid id, string comment)
        {
            var review = _context.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            
            review.Status = ReviewStatus.Rejected;
            review.ModerationComment = comment;
            _context.SaveChanges();
            
            TempData["SuccessMessage"] = "Отзыв отклонен.";
            return RedirectToAction("Moderation");
        }
        
        private Guid GetCurrentUserId()
        {
            string username = User.Identity.Name;
            var user = _context.Users.FirstOrDefault(u => u.Email == username);
            return user?.Id ?? Guid.Empty;
        }
    }
} 