using System;
using System.Collections.Generic;
using System.Linq;
using UTM.Keto.Application.Interfaces;
using UTM.Keto.Domain;
using UTM.Keto.Infrastructure;

namespace UTM.Keto.Application.BLogic
{
    public class ReviewBL : IReviewBL
    {
        private readonly ApplicationDbContext _db;

        public ReviewBL()
        {
            _db = new ApplicationDbContext();
        }

        public List<Review> GetAllReviews()
        {
            return _db.Reviews.AsQueryable()
                .OrderByDescending(r => r.CreatedDate)
                .ToList();
        }

        public List<Review> GetApprovedReviews()
        {
            return _db.Reviews.AsQueryable()
                .Where(r => r.Status == ReviewStatus.Approved)
                .OrderByDescending(r => r.CreatedDate)
                .ToList();
        }

        public List<Review> GetPendingReviews()
        {
            return _db.Reviews.AsQueryable()
                .Where(r => r.Status == ReviewStatus.PendingModeration)
                .OrderByDescending(r => r.CreatedDate)
                .ToList();
        }

        public List<Review> GetUserReviews(Guid userId)
        {
            return _db.Reviews.AsQueryable()
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedDate)
                .ToList();
        }

        public List<Review> GetUserReviews(int userId)
        {
            return _db.Reviews.AsQueryable()
                .Where(r => r.UserId.GetHashCode() == userId)
                .OrderByDescending(r => r.CreatedDate)
                .ToList();
        }

        public Review GetReviewById(Guid reviewId)
        {
            return _db.Reviews.AsQueryable()
                .FirstOrDefault(r => r.Id == reviewId);
        }

        public void CreateReview(Review review)
        {
            review.CreatedDate = DateTime.Now;
            review.Status = ReviewStatus.PendingModeration;
            
            _db.Reviews.Add(review);
            _db.SaveChanges();
        }

        public void UpdateReview(Review review)
        {
            var existingReview = _db.Reviews.AsQueryable().FirstOrDefault(r => r.Id == review.Id);
            if (existingReview != null)
            {
                existingReview.Title = review.Title;
                existingReview.Content = review.Content;
                existingReview.Rating = review.Rating;
                existingReview.ProductId = review.ProductId;
                existingReview.Status = review.Status;
                existingReview.ModerationComment = review.ModerationComment;
                
                _db.SaveChanges();
            }
        }

        public void ApproveReview(Guid reviewId)
        {
            var review = _db.Reviews.AsQueryable().FirstOrDefault(r => r.Id == reviewId);
            if (review != null)
            {
                review.Status = ReviewStatus.Approved;
                _db.SaveChanges();
            }
        }

        public void RejectReview(Guid reviewId)
        {
            var review = _db.Reviews.AsQueryable().FirstOrDefault(r => r.Id == reviewId);
            if (review != null)
            {
                review.Status = ReviewStatus.Rejected;
                _db.SaveChanges();
            }
        }

        public void DeleteReview(Guid reviewId)
        {
            var review = _db.Reviews.AsQueryable().FirstOrDefault(r => r.Id == reviewId);
            if (review != null)
            {
                _db.Reviews.Remove(review);
                _db.SaveChanges();
            }
        }
    }
} 