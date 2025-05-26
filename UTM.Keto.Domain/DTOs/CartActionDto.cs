namespace UTM.Keto.Domain.DTOs
{
    public class CartActionDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string Action { get; set; } // Add, Remove, Update
        public int? UserId { get; set; }
    }
} 