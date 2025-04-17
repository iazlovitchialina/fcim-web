using System;
using System.Linq;
using System.Web.Mvc;
using UTM.Keto.Domain;
using UTM.Keto.Infrastructure;
using UTM.Keto.Web.Models;

namespace UTM.Keto.Web.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FeedbackController()
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

        // GET: Feedback
        public ActionResult Index()
        {
            return View();
        }

        // POST: Feedback/SubmitReview
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitReview(CreateReviewViewModel model)
        {
            if (ModelState.IsValid)
            {
                Guid userId;
                
                // Если пользователь авторизован, используем его ID
                if (User.Identity.IsAuthenticated)
                {
                    userId = GetCurrentUserId();
                }
                else
                {
                    // Для анонимных пользователей создаем временный ID
                    // В реальной системе можно было бы запрашивать email/имя
                    userId = Guid.Empty;
                }
                
                var review = new Review
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Title = model.Title,
                    Content = model.Content,
                    Rating = model.Rating,
                    CreatedDate = DateTime.Now,
                    Status = ReviewStatus.PendingModeration
                };
                
                _context.Reviews.Add(review);
                _context.SaveChanges();
                
                TempData["SuccessMessage"] = "Спасибо за ваш отзыв! Он будет опубликован после проверки модератором.";
                return RedirectToAction("ThankYou");
            }
            
            return View("Index", model);
        }
        
        // GET: Feedback/SubmitTicket
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitTicket(CreateTicketViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!User.Identity.IsAuthenticated)
                {
                    // Для создания тикета нужна авторизация
                    TempData["TicketErrorMessage"] = "Для создания запроса в техподдержку необходимо авторизоваться.";
                    return RedirectToAction("Index");
                }
                
                var userId = GetCurrentUserId();
                
                var ticket = new SupportTicket
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Subject = model.Subject,
                    InitialMessage = model.Message,
                    CreatedDate = DateTime.Now,
                    Status = TicketStatus.Open
                };
                
                // Добавляем первое сообщение
                var initialMessage = new TicketMessage
                {
                    Id = Guid.NewGuid(),
                    TicketId = ticket.Id,
                    SenderId = userId,
                    Message = model.Message,
                    SentDate = DateTime.Now,
                    IsFromAdmin = false
                };
                
                ticket.Messages.Add(initialMessage);
                
                _context.SupportTickets.Add(ticket);
                _context.SaveChanges();
                
                TempData["SuccessMessage"] = "Ваш запрос успешно отправлен. Специалисты техподдержки ответят вам в ближайшее время.";
                return RedirectToAction("ThankYou");
            }
            
            return View("Index", model);
        }
        
        // GET: Feedback/ThankYou
        public ActionResult ThankYou()
        {
            return View();
        }
        
        private Guid GetCurrentUserId()
        {
            string username = User.Identity.Name;
            var user = _context.Users.FirstOrDefault(u => u.Email == username);
            return user?.Id ?? Guid.Empty;
        }
    }
} 