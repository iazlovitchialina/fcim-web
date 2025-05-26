using System;
using System.Collections.Generic;
using System.Linq;
using UTM.Keto.Application.Interfaces;
using UTM.Keto.Domain;
using UTM.Keto.Infrastructure;

namespace UTM.Keto.Application.BLogic
{
    public class SupportBL : ISupportBL
    {
        private readonly ApplicationDbContext _db;

        public SupportBL()
        {
            _db = new ApplicationDbContext();
        }

        public List<SupportTicket> GetAllTickets()
        {
            return _db.SupportTickets.AsQueryable()
                .OrderByDescending(t => t.CreatedDate)
                .ToList();
        }

        public List<SupportTicket> GetUserTickets(Guid userId)
        {
            return _db.SupportTickets.AsQueryable()
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedDate)
                .ToList();
        }

        public List<SupportTicket> GetUserTickets(int userId)
        {
            return _db.SupportTickets.AsQueryable()
                .Where(t => t.UserId.GetHashCode() == userId)
                .OrderByDescending(t => t.CreatedDate)
                .ToList();
        }

        public SupportTicket GetTicketById(Guid ticketId)
        {
            return _db.SupportTickets.AsQueryable()
                .FirstOrDefault(t => t.Id == ticketId);
        }

        public SupportTicket CreateTicket(SupportTicket ticket)
        {
            ticket.CreatedDate = DateTime.Now;
            ticket.Status = TicketStatus.Open;
            
            _db.SupportTickets.Add(ticket);
            _db.SaveChanges();
            
            return ticket;
        }

        public void UpdateTicket(SupportTicket ticket)
        {
            var existingTicket = _db.SupportTickets.AsQueryable().FirstOrDefault(t => t.Id == ticket.Id);
            if (existingTicket != null)
            {
                existingTicket.Subject = ticket.Subject;
                existingTicket.InitialMessage = ticket.InitialMessage;
                existingTicket.Status = ticket.Status;
                existingTicket.Priority = ticket.Priority;
                
                _db.SaveChanges();
            }
        }

        public void AddMessage(TicketMessage message)
        {
            message.DateSent = DateTime.Now;
            
            _db.TicketMessages.Add(message);
            _db.SaveChanges();
            
            // Обновляем статус тикета, если это сообщение от администратора
            var ticket = _db.SupportTickets.AsQueryable().FirstOrDefault(t => t.Id == message.TicketId);
            if (ticket != null && message.IsFromAdmin && ticket.Status == TicketStatus.Open)
            {
                ticket.Status = TicketStatus.InProgress;
                _db.SaveChanges();
            }
        }

        public void CloseTicket(Guid ticketId)
        {
            var ticket = _db.SupportTickets.AsQueryable().FirstOrDefault(t => t.Id == ticketId);
            if (ticket != null)
            {
                ticket.Status = TicketStatus.Closed;
                ticket.ClosedDate = DateTime.Now;
                _db.SaveChanges();
            }
        }

        public void ReopenTicket(Guid ticketId)
        {
            var ticket = _db.SupportTickets.AsQueryable().FirstOrDefault(t => t.Id == ticketId);
            if (ticket != null && ticket.Status == TicketStatus.Closed)
            {
                ticket.Status = TicketStatus.Open;
                ticket.ClosedDate = null;
                _db.SaveChanges();
            }
        }
    }
} 