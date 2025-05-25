using System;
using System.Collections.Generic;
using System.Linq;
using UTM.Keto.Application.Interfaces;
using UTM.Keto.Domain;
using UTM.Keto.Infrastructure;

namespace UTM.Keto.Application.BLogic
{
    public class ProductBL : IProductBL
    {
        private readonly ApplicationDbContext _db;

        public ProductBL()
        {
            _db = new ApplicationDbContext();
        }

        public List<Product> GetAllProducts()
        {
            return _db.Products.ToList();
        }

        public Product GetProductById(int productId)
        {
            return _db.Products.AsQueryable().FirstOrDefault(p => p.Id.GetHashCode() == productId);
        }

        public Product GetProductById(Guid productId)
        {
            return _db.Products.AsQueryable().FirstOrDefault(p => p.Id == productId);
        }

        public void AddProduct(Product product)
        {
            _db.Products.Add(product);
            _db.SaveChanges();
        }

        public void UpdateProduct(Product product)
        {
            var existingProduct = _db.Products.AsQueryable().FirstOrDefault(p => p.Id == product.Id);
            if (existingProduct != null)
            {
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                _db.SaveChanges();
            }
        }

        public void DeleteProduct(int productId)
        {
            var product = _db.Products.AsQueryable().FirstOrDefault(p => p.Id.GetHashCode() == productId);
            if (product != null)
            {
                _db.Products.Remove(product);
                _db.SaveChanges();
            }
        }

        public void DeleteProduct(Guid productId)
        {
            var product = _db.Products.AsQueryable().FirstOrDefault(p => p.Id == productId);
            if (product != null)
            {
                _db.Products.Remove(product);
                _db.SaveChanges();
            }
        }
    }
} 