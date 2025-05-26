using System;
using System.Collections.Generic;
using UTM.Keto.Domain;

namespace UTM.Keto.Application.Interfaces
{
    public interface IReviewBL
    {
        List<Review> GetAllReviews();
        List<Review> GetApprovedReviews();
        List<Review> GetPendingReviews();
        List<Review> GetUserReviews(Guid userId);
        List<Review> GetUserReviews(int userId);
        Review GetReviewById(Guid reviewId);
        void CreateReview(Review review);
        void UpdateReview(Review review);
        void ApproveReview(Guid reviewId);
        void RejectReview(Guid reviewId);
        void DeleteReview(Guid reviewId);
    }
} 