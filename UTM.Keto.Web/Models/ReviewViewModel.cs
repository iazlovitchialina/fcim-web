using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UTM.Keto.Domain;

namespace UTM.Keto.Web.Models
{
    public class ReviewViewModel
    {
        public Guid Id { get; set; }
        
        public string UserName { get; set; }
        
        public string Title { get; set; }
        
        public string Content { get; set; }
        
        public int Rating { get; set; }
        
        public DateTime CreatedDate { get; set; }
        
        public string Status { get; set; }
        
        public ReviewStatus CurrentStatus { get; set; }
        
        public string ModerationComment { get; set; }
        
        public string ProductName { get; set; }
        
        public Guid? ProductId { get; set; }
    }
    
    public class CreateReviewViewModel
    {
        [Required(ErrorMessage = "Заголовок обязателен")]
        [StringLength(100, ErrorMessage = "Заголовок не может превышать 100 символов")]
        [Display(Name = "Заголовок")]
        public string Title { get; set; }
        
        [Required(ErrorMessage = "Текст отзыва обязателен")]
        [Display(Name = "Текст отзыва")]
        public string Content { get; set; }
        
        [Required(ErrorMessage = "Оценка обязательна")]
        [Range(1, 5, ErrorMessage = "Оценка должна быть от 1 до 5")]
        [Display(Name = "Оценка")]
        public int Rating { get; set; }
        
        public Guid? ProductId { get; set; }
    }
    
    public class ModerateReviewViewModel
    {
        public Guid ReviewId { get; set; }
        
        [Display(Name = "Комментарий модератора")]
        public string ModerationComment { get; set; }
        
        public ReviewStatus NewStatus { get; set; }
    }
} 