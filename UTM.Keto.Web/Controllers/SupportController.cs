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
    public class SupportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SupportController()
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

        // GET: Support
        [Authorize]
        public ActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                // Администраторы видят все тикеты
                var allTickets = _context.SupportTickets
                    .OrderByDescending(t => t.CreatedDate)
                    .Include(t => t.User)
                    .ToList();
                    
                var viewModels = allTickets.Select(t => new TicketViewModel
                {
                    Id = t.Id,
                    TicketNumber = t.TicketNumber,
                    UserName = t.User.FullName,
                    Subject = t.Subject,
                    InitialMessage = t.InitialMessage,
                    CreatedDate = t.CreatedDate,
                    ClosedDate = t.ClosedDate,
                    Status = t.Status.ToString(),
                    CurrentStatus = t.Status
                }).ToList();
                
                return View(viewModels);
            }
            else
            {
                // Обычные пользователи видят только свои тикеты
                var userId = GetCurrentUserId();
                var userTickets = _context.SupportTickets
                    .Where(t => t.UserId == userId)
                    .OrderByDescending(t => t.CreatedDate)
                    .ToList();
                    
                var viewModels = userTickets.Select(t => new TicketViewModel
                {
                    Id = t.Id,
                    TicketNumber = t.TicketNumber,
                    Subject = t.Subject,
                    InitialMessage = t.InitialMessage,
                    CreatedDate = t.CreatedDate,
                    ClosedDate = t.ClosedDate,
                    Status = t.Status.ToString(),
                    CurrentStatus = t.Status
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
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Subject = model.Subject,
                    InitialMessage = model.Message,
                    CreatedDate = DateTime.Now,
                    Status = TicketStatus.Open
                };
                
                // Добавляем первое сообщение (начальное)
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
                
                TempData["SuccessMessage"] = "Тикет успешно создан. Номер тикета: " + ticket.TicketNumber;
                return RedirectToAction("Details", new { id = ticket.Id });
            }
            
            return View(model);
        }

        // GET: Support/Details/5
        [Authorize]
        public ActionResult Details(Guid id)
        {
            var ticket = _context.SupportTickets
                .Include(t => t.User)
                .Include(t => t.Messages.Select(m => m.Sender))
                .FirstOrDefault(t => t.Id == id);
                
            if (ticket == null)
            {
                return HttpNotFound();
            }
            
            // Проверяем доступ
            if (!User.IsInRole("Admin") && ticket.UserId != GetCurrentUserId())
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
            }
            
            var viewModel = new TicketViewModel
            {
                Id = ticket.Id,
                TicketNumber = ticket.TicketNumber,
                UserName = ticket.User.FullName,
                Subject = ticket.Subject,
                InitialMessage = ticket.InitialMessage,
                CreatedDate = ticket.CreatedDate,
                ClosedDate = ticket.ClosedDate,
                Status = ticket.Status.ToString(),
                CurrentStatus = ticket.Status,
                Messages = ticket.Messages
                    .OrderBy(m => m.SentDate)
                    .Select(m => new TicketMessageViewModel
                    {
                        Id = m.Id,
                        SenderName = m.Sender.FullName,
                        Message = m.Message,
                        SentDate = m.SentDate,
                        IsFromAdmin = m.IsFromAdmin
                    }).ToList()
            };
            
            return View(viewModel);
        }

        // GET: Support/Reply/5
        [Authorize]
        public ActionResult Reply(Guid id)
        {
            var ticket = _context.SupportTickets.Find(id);
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
                var ticket = _context.SupportTickets.Find(model.TicketId);
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
                    Id = Guid.NewGuid(),
                    TicketId = ticket.Id,
                    SenderId = userId,
                    Message = model.Message,
                    SentDate = DateTime.Now,
                    IsFromAdmin = User.IsInRole("Admin")
                };
                
                _context.TicketMessages.Add(message);
                
                // Если тикет открыт и отвечает админ, переводим в статус "In Progress"
                if (ticket.Status == TicketStatus.Open && User.IsInRole("Admin"))
                {
                    ticket.Status = TicketStatus.InProgress;
                }
                
                _context.SaveChanges();
                
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
            var ticket = _context.SupportTickets.Find(id);
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
                ticket.Status = TicketStatus.Closed;
                ticket.ClosedDate = DateTime.Now;
                _context.SaveChanges();
                
                TempData["SuccessMessage"] = "Тикет успешно закрыт.";
            }
            
            return RedirectToAction("Details", new { id = ticket.Id });
        }
        
        private Guid GetCurrentUserId()
        {
            string username = User.Identity.Name;
            var user = _context.Users.FirstOrDefault(u => u.Email == username);
            return user?.Id ?? Guid.Empty;
        }
    }
} 