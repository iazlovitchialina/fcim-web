using System;
using System.ComponentModel.DataAnnotations;

namespace UTM.Keto.Web.Models
{
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
} 