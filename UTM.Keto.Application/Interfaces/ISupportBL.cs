using System;
using System.Collections.Generic;
using UTM.Keto.Domain;

namespace UTM.Keto.Application.Interfaces
{
    public interface ISupportBL
    {
        List<SupportTicket> GetAllTickets();
        List<SupportTicket> GetUserTickets(Guid userId);
        List<SupportTicket> GetUserTickets(int userId);
        SupportTicket GetTicketById(Guid ticketId);
        SupportTicket CreateTicket(SupportTicket ticket);
        void UpdateTicket(SupportTicket ticket);
        void AddMessage(TicketMessage message);
        void CloseTicket(Guid ticketId);
        void ReopenTicket(Guid ticketId);
    }
} 