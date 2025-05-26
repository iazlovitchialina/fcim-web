using System;
using System.Collections.Generic;
using UTM.Keto.Domain;

namespace UTM.Keto.Application.Interfaces
{
    public interface IFeedbackBL
    {
        List<Review> GetAllFeedback();
        Review GetFeedbackById(Guid feedbackId);
        void CreateFeedback(Review feedback);
        void UpdateFeedback(Review feedback);
        void DeleteFeedback(Guid feedbackId);
    }
} 