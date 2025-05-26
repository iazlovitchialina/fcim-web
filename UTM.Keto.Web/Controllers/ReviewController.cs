using System;
using System.Linq;
using System.Web.Mvc;
using UTM.Keto.Application;
using UTM.Keto.Application.Interfaces;
using UTM.Keto.Domain;
using UTM.Keto.Web.Filters;
using UTM.Keto.Web.Models;

namespace UTM.Keto.Web.Controllers
{
    public class ReviewController : Controller
    {
        private readonly IReviewBL _reviewBL;
        private readonly IUserBL _userBL;
        private readonly IProductBL _productBL;

        public ReviewController()
        {
            var factory = BusinessLogicFactory.Instance;
            _reviewBL = factory.GetReviewBL();
            _userBL = factory.GetUserBL();
            _productBL = factory.GetProductBL();
        }

        // GET: Review
        public ActionResult Index()
        {
            var approvedReviews = _reviewBL.GetApprovedReviews();
                
            var viewModels = approvedReviews.Select(r => new ReviewViewModel
            {
                Id = r.Id,
                UserName = _userBL.GetUserById(r.UserId).FullName,
                Title = r.Title,
                Content = r.Content,
                Rating = r.Rating,
                CreatedDate = r.CreatedDate,
                Status = r.Status.ToString(),
                CurrentStatus = r.Status.ToString(),
                ProductName = r.ProductId.HasValue ? _productBL.GetProductById(r.ProductId.Value.GetHashCode()).Name : null,
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
                var product = _productBL.GetProductById(productId.Value.GetHashCode());
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
                    UserId = userId,
                    Title = model.Title,
                    Content = model.Content,
                    Rating = model.Rating,
                    ProductId = model.ProductId
                };
                
                _reviewBL.CreateReview(review);
                
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
            var review = _reviewBL.GetReviewById(id);
                
            if (review == null)
            {
                return HttpNotFound();
            }
            
            var user = _userBL.GetUserById(review.UserId);
            var product = review.ProductId.HasValue ? _productBL.GetProductById(review.ProductId.Value.GetHashCode()) : null;
            
            var viewModel = new ReviewViewModel
            {
                Id = review.Id,
                UserName = user.FullName,
                Title = review.Title,
                Content = review.Content,
                Rating = review.Rating,
                CreatedDate = review.CreatedDate,
                Status = review.Status.ToString(),
                CurrentStatus = review.Status.ToString(),
                ModerationComment = review.ModerationComment,
                ProductName = product?.Name,
                ProductId = review.ProductId
            };
            
            return View(viewModel);
        }
        
        // GET: Review/Moderation
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Moderation()
        {
            var pendingReviews = _reviewBL.GetPendingReviews();
                
            var viewModels = pendingReviews.Select(r => new ReviewViewModel
            {
                Id = r.Id,
                UserName = _userBL.GetUserById(r.UserId).FullName,
                Title = r.Title,
                Content = r.Content,
                Rating = r.Rating,
                CreatedDate = r.CreatedDate,
                Status = r.Status.ToString(),
                CurrentStatus = r.Status.ToString(),
                ProductName = r.ProductId.HasValue ? _productBL.GetProductById(r.ProductId.Value.GetHashCode()).Name : null,
                ProductId = r.ProductId
            }).ToList();
            
            return View(viewModels);
        }
        
        // GET: Review/Moderate/5
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Moderate(Guid id)
        {
            var review = _reviewBL.GetReviewById(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            
            var model = new ModerateReviewViewModel
            {
                ReviewId = review.Id,
                ModerationComment = review.ModerationComment,
                NewStatus = review.Status.ToString()
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
                var review = _reviewBL.GetReviewById(model.ReviewId);
                if (review == null)
                {
                    return HttpNotFound();
                }
                
                // Конвертируем строковый статус в enum
                ReviewStatus newStatus;
                if (Enum.TryParse<ReviewStatus>(model.NewStatus, out newStatus))
                {
                    review.Status = newStatus;
                }
                
                review.ModerationComment = model.ModerationComment;
                
                _reviewBL.UpdateReview(review);
                
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
            var review = _reviewBL.GetReviewById(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            
            _reviewBL.ApproveReview(id);
            
            TempData["SuccessMessage"] = "Отзыв одобрен и опубликован.";
            return RedirectToAction("Moderation");
        }
        
        // POST: Review/Reject/5
        [HttpPost]
        [CustomAuthorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Reject(Guid id, string comment)
        {
            var review = _reviewBL.GetReviewById(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            
            review.Status = ReviewStatus.Rejected;
            review.ModerationComment = comment;
            
            _reviewBL.UpdateReview(review);
            
            TempData["SuccessMessage"] = "Отзыв отклонен.";
            return RedirectToAction("Moderation");
        }
        
        private Guid GetCurrentUserId()
        {
            var ticket = System.Web.Security.FormsAuthentication.Decrypt(Request.Cookies[System.Web.Security.FormsAuthentication.FormsCookieName].Value);
            var userData = ticket.UserData.Split('|');
            return new Guid(userData[0]);
        }
    }
} 