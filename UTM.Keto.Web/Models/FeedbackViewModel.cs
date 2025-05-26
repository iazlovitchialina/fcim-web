using System;
using System.Collections.Generic;
using UTM.Keto.Domain;

namespace UTM.Keto.Web.Models
{
    public class FeedbackViewModel
    {
        public List<ReviewViewModel> Feedbacks { get; set; }
        
        public CreateReviewViewModel ReviewForm { get; set; }
        
        public CreateTicketViewModel TicketForm { get; set; }
        
        public FeedbackViewModel()
        {
            Feedbacks = new List<ReviewViewModel>();
            ReviewForm = new CreateReviewViewModel();
            TicketForm = new CreateTicketViewModel();
        }
    }
} 