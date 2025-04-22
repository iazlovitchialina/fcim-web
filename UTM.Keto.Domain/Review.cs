using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UTM.Keto.Domain
{
    [Table("Reviews")]
    public class Review : BaseEntity
    {
        [Required]
        public Guid UserId { get; set; }
        
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        
        [Required]
        public string Title { get; set; }
        
        [Required]
        public string Content { get; set; }
        
        [Required]
        public int Rating { get; set; }
        
        [Required]
        public DateTime CreatedDate { get; set; }
        
        [Required]
        public ReviewStatus Status { get; set; }
        
        public string ModerationComment { get; set; }
        
        // Если отзыв относится к товару
        public Guid? ProductId { get; set; }
        
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
        
        public Review()
        {
            CreatedDate = DateTime.Now;
            Status = ReviewStatus.PendingModeration;
        }
    }
    
    public enum ReviewStatus
    {
        PendingModeration,
        Approved,
        Rejected
    }
} 