using System;
using UTM.Keto.Domain;
using UTM.Keto.Domain.DTOs;
using System.Collections.Generic;

namespace UTM.Keto.Application.Interfaces
{
    public interface ICartBL
    {
        CartResultDto AddToCart(CartActionDto action);
        CartResultDto RemoveFromCart(CartActionDto action);
        CartResultDto UpdateCartItemQuantity(CartActionDto action);
        CartResultDto GetCart(int userId);
        CartResultDto GetCart(Guid userId);
        void ClearCart(int userId);
        void ClearCart(Guid userId);
    }
} 