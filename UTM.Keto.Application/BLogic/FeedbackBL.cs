using System;
using System.Collections.Generic;
using System.Linq;
using UTM.Keto.Application.Interfaces;
using UTM.Keto.Domain;
using UTM.Keto.Infrastructure;

namespace UTM.Keto.Application.BLogic
{
    public class FeedbackBL : IFeedbackBL
    {
        private readonly ApplicationDbContext _db;

        public FeedbackBL()
        {
            _db = new ApplicationDbContext();
        }

        public List<Review> GetAllFeedback()
        {
            return _db.Reviews.AsQueryable()
                .Where(r => r.Type == "Feedback")
                .OrderByDescending(r => r.CreatedDate)
                .ToList();
        }

        public Review GetFeedbackById(Guid feedbackId)
        {
            return _db.Reviews.AsQueryable()
                .FirstOrDefault(r => r.Id == feedbackId && r.Type == "Feedback");
        }

        public void CreateFeedback(Review feedback)
        {
            feedback.CreatedDate = DateTime.Now;
            feedback.Type = "Feedback";
            feedback.Status = ReviewStatus.PendingModeration;
            
            _db.Reviews.Add(feedback);
            _db.SaveChanges();
        }

        public void UpdateFeedback(Review feedback)
        {
            var existingFeedback = _db.Reviews.AsQueryable()
                .FirstOrDefault(r => r.Id == feedback.Id && r.Type == "Feedback");
                
            if (existingFeedback != null)
            {
                existingFeedback.Title = feedback.Title;
                existingFeedback.Content = feedback.Content;
                existingFeedback.Status = feedback.Status;
                
                _db.SaveChanges();
            }
        }

        public void DeleteFeedback(Guid feedbackId)
        {
            var feedback = _db.Reviews.AsQueryable()
                .FirstOrDefault(r => r.Id == feedbackId && r.Type == "Feedback");
                
            if (feedback != null)
            {
                _db.Reviews.Remove(feedback);
                _db.SaveChanges();
            }
        }
    }
} 