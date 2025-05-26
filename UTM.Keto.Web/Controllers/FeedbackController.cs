using System;
using System.Linq;
using System.Web.Mvc;
using UTM.Keto.Application;
using UTM.Keto.Application.Interfaces;
using UTM.Keto.Domain;
using UTM.Keto.Web.Models;

namespace UTM.Keto.Web.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly IFeedbackBL _feedbackBL;
        private readonly ISupportBL _supportBL;
        private readonly IUserBL _userBL;

        public FeedbackController()
        {
            var factory = BusinessLogicFactory.Instance;
            _feedbackBL = factory.GetFeedbackBL();
            _supportBL = factory.GetSupportBL();
            _userBL = factory.GetUserBL();
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
                    UserId = userId,
                    Title = model.Title,
                    Content = model.Content,
                    Rating = model.Rating,
                    Type = "Feedback"
                };
                
                _feedbackBL.CreateFeedback(review);
                
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
                    UserId = userId,
                    Subject = model.Subject,
                    InitialMessage = model.Message,
                    Priority = "Normal"
                };
                
                var createdTicket = _supportBL.CreateTicket(ticket);
                
                // Добавляем первое сообщение
                var initialMessage = new TicketMessage
                {
                    TicketId = createdTicket.Id,
                    UserId = userId,
                    Content = model.Message,
                    IsFromAdmin = false
                };
                
                _supportBL.AddMessage(initialMessage);
                
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
            var ticket = System.Web.Security.FormsAuthentication.Decrypt(Request.Cookies[System.Web.Security.FormsAuthentication.FormsCookieName].Value);
            var userData = ticket.UserData.Split('|');
            return new Guid(userData[0]);
        }
    }
} 