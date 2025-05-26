using System.Collections.Generic;

namespace UTM.Keto.Domain.DTOs
{
    public class CartResultDto
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
        public decimal TotalPrice { get; set; }

        public class CartItemDto
        {
            public int Id { get; set; }
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }
            public decimal SubTotal { get; set; }
        }
    }
} 