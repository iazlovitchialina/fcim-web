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
    public class SupportController : Controller
    {
        private readonly ISupportBL _supportBL;
        private readonly IUserBL _userBL;

        public SupportController()
        {
            var factory = BusinessLogicFactory.Instance;
            _supportBL = factory.GetSupportBL();
            _userBL = factory.GetUserBL();
        }

        // GET: Support
        [Authorize]
        public ActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                // Администраторы видят все тикеты
                var allTickets = _supportBL.GetAllTickets();
                    
                var viewModels = allTickets.Select(t => new TicketViewModel
                {
                    Id = t.Id,
                    TicketNumber = t.TicketNumber,
                    UserName = _userBL.GetUserById(t.UserId).FullName,
                    Subject = t.Subject,
                    InitialMessage = t.InitialMessage,
                    CreatedDate = t.CreatedDate,
                    ClosedDate = t.ClosedDate,
                    Status = t.Status.ToString(),
                    CurrentStatus = t.Status.ToString()
                }).ToList();
                
                return View(viewModels);
            }
            else
            {
                // Обычные пользователи видят только свои тикеты
                var userId = GetCurrentUserId();
                var userTickets = _supportBL.GetUserTickets(userId);
                    
                var viewModels = userTickets.Select(t => new TicketViewModel
                {
                    Id = t.Id,
                    TicketNumber = t.TicketNumber,
                    Subject = t.Subject,
                    InitialMessage = t.InitialMessage,
                    CreatedDate = t.CreatedDate,
                    ClosedDate = t.ClosedDate,
                    Status = t.Status.ToString(),
                    CurrentStatus = t.Status.ToString()
                }).ToList();
                
                return View(viewModels);
            }
        }

        // GET: Support/Create
        [Authorize]
        public ActionResult Create()
        {
            return View(new CreateTicketViewModel());
        }

        // POST: Support/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateTicketViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = GetCurrentUserId();
                
                var ticket = new SupportTicket
                {
                    UserId = userId,
                    Subject = model.Subject,
                    InitialMessage = model.Message,
                    Priority = "Normal"
                };
                
                var createdTicket = _supportBL.CreateTicket(ticket);
                
                // Добавляем первое сообщение (начальное)
                var initialMessage = new TicketMessage
                {
                    TicketId = createdTicket.Id,
                    UserId = userId,
                    Content = model.Message,
                    IsFromAdmin = false
                };
                
                _supportBL.AddMessage(initialMessage);
                
                TempData["SuccessMessage"] = "Тикет успешно создан. Номер тикета: " + createdTicket.TicketNumber;
                return RedirectToAction("Details", new { id = createdTicket.Id });
            }
            
            return View(model);
        }

        // GET: Support/Details/5
        [Authorize]
        public ActionResult Details(Guid id)
        {
            var ticket = _supportBL.GetTicketById(id);
                
            if (ticket == null)
            {
                return HttpNotFound();
            }
            
            // Проверяем доступ
            if (!User.IsInRole("Admin") && ticket.UserId != GetCurrentUserId())
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
            }
            
            var user = _userBL.GetUserById(ticket.UserId);
            
            var viewModel = new TicketViewModel
            {
                Id = ticket.Id,
                TicketNumber = ticket.TicketNumber,
                UserName = user.FullName,
                Subject = ticket.Subject,
                InitialMessage = ticket.InitialMessage,
                CreatedDate = ticket.CreatedDate,
                ClosedDate = ticket.ClosedDate,
                Status = ticket.Status.ToString(),
                CurrentStatus = ticket.Status.ToString(),
                Messages = ticket.Messages
                    .OrderBy(m => m.DateSent)
                    .Select(m => new TicketMessageViewModel
                    {
                        Id = m.Id,
                        SenderName = _userBL.GetUserById(m.UserId).FullName,
                        Message = m.Content,
                        SentDate = m.DateSent,
                        IsFromAdmin = m.IsFromAdmin
                    }).ToList()
            };
            
            return View(viewModel);
        }

        // GET: Support/Reply/5
        [Authorize]
        public ActionResult Reply(Guid id)
        {
            var ticket = _supportBL.GetTicketById(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            
            // Проверяем доступ
            if (!User.IsInRole("Admin") && ticket.UserId != GetCurrentUserId())
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
            }
            
            // Нельзя отвечать на закрытые тикеты
            if (ticket.Status == TicketStatus.Closed)
            {
                TempData["ErrorMessage"] = "Невозможно ответить на закрытый тикет.";
                return RedirectToAction("Details", new { id = ticket.Id });
            }
            
            var model = new ReplyTicketViewModel
            {
                TicketId = ticket.Id
            };
            
            return View(model);
        }

        // POST: Support/Reply
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Reply(ReplyTicketViewModel model)
        {
            if (ModelState.IsValid)
            {
                var ticket = _supportBL.GetTicketById(model.TicketId);
                if (ticket == null)
                {
                    return HttpNotFound();
                }
                
                // Проверяем доступ
                if (!User.IsInRole("Admin") && ticket.UserId != GetCurrentUserId())
                {
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
                }
                
                // Нельзя отвечать на закрытые тикеты
                if (ticket.Status == TicketStatus.Closed)
                {
                    TempData["ErrorMessage"] = "Невозможно ответить на закрытый тикет.";
                    return RedirectToAction("Details", new { id = ticket.Id });
                }
                
                var userId = GetCurrentUserId();
                
                // Создаем новое сообщение
                var message = new TicketMessage
                {
                    TicketId = ticket.Id,
                    UserId = userId,
                    Content = model.Message,
                    IsFromAdmin = User.IsInRole("Admin")
                };
                
                _supportBL.AddMessage(message);
                
                TempData["SuccessMessage"] = "Ответ успешно отправлен.";
                return RedirectToAction("Details", new { id = ticket.Id });
            }
            
            return View(model);
        }

        // POST: Support/Close/5
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Close(Guid id)
        {
            var ticket = _supportBL.GetTicketById(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            
            // Только админы или владелец тикета могут закрыть его
            if (!User.IsInRole("Admin") && ticket.UserId != GetCurrentUserId())
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
            }
            
            // Пропускаем, если тикет уже закрыт
            if (ticket.Status != TicketStatus.Closed)
            {
                _supportBL.CloseTicket(id);
                TempData["SuccessMessage"] = "Тикет успешно закрыт.";
            }
            
            return RedirectToAction("Details", new { id = ticket.Id });
        }
        
        private Guid GetCurrentUserId()
        {
            var ticket = System.Web.Security.FormsAuthentication.Decrypt(Request.Cookies[System.Web.Security.FormsAuthentication.FormsCookieName].Value);
            var userData = ticket.UserData.Split('|');
            return new Guid(userData[0]);
        }
    }
} 