using System;
using System.Collections.Generic;
using System.Linq;
using UTM.Keto.Application.Interfaces;
using UTM.Keto.Domain;
using UTM.Keto.Domain.DTOs;
using UTM.Keto.Infrastructure;

namespace UTM.Keto.Application.BLogic
{
    public class CartBL : ICartBL
    {
        private readonly ApplicationDbContext _db;
        private readonly IProductBL _productBL;

        public CartBL()
        {
            _db = new ApplicationDbContext();
            _productBL = BusinessLogicFactory.Instance.GetProductBL();
        }

        public CartResultDto AddToCart(CartActionDto action)
        {
            var result = new CartResultDto { IsSuccess = false };

            try
            {
                var product = _productBL.GetProductById(action.ProductId);
                if (product == null)
                {
                    result.ErrorMessage = "Продукт не найден";
                    return result;
                }

                var cartItem = _db.CartItems.AsQueryable()
                    .FirstOrDefault(ci => ci.UserId.GetHashCode() == action.UserId && 
                                         ci.ProductId.GetHashCode() == action.ProductId);
                if (cartItem == null)
                {
                    cartItem = new CartItem
                    {
                        UserId = new Guid(action.UserId.Value.ToString()),
                        ProductId = new Guid(action.ProductId.ToString()),
                        Quantity = action.Quantity
                    };
                    _db.CartItems.Add(cartItem);
                }
                else
                {
                    cartItem.Quantity += action.Quantity;
                }

                _db.SaveChanges();
                return GetCart(action.UserId.Value);
            }
            catch (Exception ex)
            {
                result.ErrorMessage = "Ошибка при добавлении товара в корзину: " + ex.Message;
                return result;
            }
        }

        public CartResultDto RemoveFromCart(CartActionDto action)
        {
            var result = new CartResultDto { IsSuccess = false };

            try
            {
                var cartItem = _db.CartItems.AsQueryable()
                    .FirstOrDefault(ci => ci.UserId.GetHashCode() == action.UserId && 
                                         ci.ProductId.GetHashCode() == action.ProductId);
                if (cartItem == null)
                {
                    result.ErrorMessage = "Товар не найден в корзине";
                    return result;
                }

                _db.CartItems.Remove(cartItem);
                _db.SaveChanges();
                return GetCart(action.UserId.Value);
            }
            catch (Exception ex)
            {
                result.ErrorMessage = "Ошибка при удалении товара из корзины: " + ex.Message;
                return result;
            }
        }

        public CartResultDto UpdateCartItemQuantity(CartActionDto action)
        {
            var result = new CartResultDto { IsSuccess = false };

            try
            {
                var cartItem = _db.CartItems.AsQueryable()
                    .FirstOrDefault(ci => ci.UserId.GetHashCode() == action.UserId && 
                                         ci.ProductId.GetHashCode() == action.ProductId);
                if (cartItem == null)
                {
                    result.ErrorMessage = "Товар не найден в корзине";
                    return result;
                }

                cartItem.Quantity = action.Quantity;
                if (cartItem.Quantity <= 0)
                {
                    _db.CartItems.Remove(cartItem);
                }

                _db.SaveChanges();
                return GetCart(action.UserId.Value);
            }
            catch (Exception ex)
            {
                result.ErrorMessage = "Ошибка при обновлении количества товара: " + ex.Message;
                return result;
            }
        }

        public CartResultDto GetCart(int userId)
        {
            var result = new CartResultDto { IsSuccess = true };

            var cartItems = _db.CartItems.AsQueryable()
                .Where(ci => ci.UserId.GetHashCode() == userId)
                .ToList();
            decimal totalPrice = 0;

            foreach (var item in cartItems)
            {
                var product = _productBL.GetProductById(item.ProductId.GetHashCode());
                if (product != null)
                {
                    var cartItemDto = new CartResultDto.CartItemDto
                    {
                        Id = item.Id.GetHashCode(),
                        ProductId = item.ProductId.GetHashCode(),
                        ProductName = product.Name,
                        Price = product.Price,
                        Quantity = item.Quantity,
                        SubTotal = product.Price * item.Quantity
                    };
                    
                    result.Items.Add(cartItemDto);
                    totalPrice += cartItemDto.SubTotal;
                }
            }

            result.TotalPrice = totalPrice;
            return result;
        }

        public CartResultDto GetCart(Guid userId)
        {
            return GetCart(userId.GetHashCode());
        }

        public void ClearCart(int userId)
        {
            var cartItems = _db.CartItems.AsQueryable()
                .Where(ci => ci.UserId.GetHashCode() == userId)
                .ToList();
            foreach (var item in cartItems)
            {
                _db.CartItems.Remove(item);
            }
            _db.SaveChanges();
        }

        public void ClearCart(Guid userId)
        {
            ClearCart(userId.GetHashCode());
        }
    }
} 