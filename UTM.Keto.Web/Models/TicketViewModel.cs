using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UTM.Keto.Domain;

namespace UTM.Keto.Web.Models
{
    public class TicketViewModel
    {
        public Guid Id { get; set; }
        public string TicketNumber { get; set; }
        public string UserName { get; set; }
        public string Subject { get; set; }
        public string InitialMessage { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public string Status { get; set; }
        public string CurrentStatus { get; set; }
        public List<TicketMessageViewModel> Messages { get; set; }
        
        public TicketViewModel()
        {
            Messages = new List<TicketMessageViewModel>();
        }
    }
    
    public class TicketMessageViewModel
    {
        public Guid Id { get; set; }
        public string SenderName { get; set; }
        public string Message { get; set; }
        public DateTime SentDate { get; set; }
        public bool IsFromAdmin { get; set; }
    }
    
    public class CreateTicketViewModel
    {
        [Required(ErrorMessage = "Тема обязательна")]
        [StringLength(100, ErrorMessage = "Тема не может превышать 100 символов")]
        [Display(Name = "Тема")]
        public string Subject { get; set; }
        
        [Required(ErrorMessage = "Сообщение обязательно")]
        [Display(Name = "Сообщение")]
        public string Message { get; set; }
    }
    
    public class ReplyTicketViewModel
    {
        public Guid TicketId { get; set; }
        
        [Required(ErrorMessage = "Сообщение обязательно")]
        [Display(Name = "Сообщение")]
        public string Message { get; set; }
    }
} 