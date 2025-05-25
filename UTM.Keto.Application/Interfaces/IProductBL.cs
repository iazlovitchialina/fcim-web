using System;
using System.Collections.Generic;
using UTM.Keto.Domain;

namespace UTM.Keto.Application.Interfaces
{
    public interface IProductBL
    {
        List<Product> GetAllProducts();
        Product GetProductById(int productId);
        Product GetProductById(Guid productId);
        void AddProduct(Product product);
        void UpdateProduct(Product product);
        void DeleteProduct(int productId);
        void DeleteProduct(Guid productId);
    }
} 