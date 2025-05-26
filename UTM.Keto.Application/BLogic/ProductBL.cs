using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public List<Product> GetFeaturedProducts()
        {
            IQueryable<Product> query = _db.Products;
            return query.Where<Product>(p => p.IsFeatured)
                .OrderBy<Product, string>(p => p.Name)
                .Take<Product>(6)
                .ToList<Product>();
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
                existingProduct.IsFeatured = product.IsFeatured;
                existingProduct.InStock = product.InStock;
                existingProduct.ImagePath = product.ImagePath;
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