using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UTM.Keto.Domain;
using UTM.Keto.Infrastructure;
using UTM.Keto.Web.Filters;

namespace UTM.Keto.Web.Controllers
{
    [CustomAuthorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }

        // GET: Product
        public ActionResult Index()
        {
            var products = _context.Products.ToList();
            return View(products);
        }

        // GET: Product/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(imageFile.FileName);
                    var fileExtension = Path.GetExtension(fileName);
                    var newFileName = Guid.NewGuid().ToString() + fileExtension;
                    var path = Path.Combine(Server.MapPath("~/Content/ProductImages"), newFileName);
                    
                    // Ensure directory exists
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    
                    imageFile.SaveAs(path);
                    product.ImagePath = "/Content/ProductImages/" + newFileName;
                }
                
                product.Id = Guid.NewGuid();
                _context.Products.Add(product);
                _context.SaveChanges();
                
                return RedirectToAction("Index");
            }
            
            return View(product);
        }

        // GET: Product/Edit/5
        public ActionResult Edit(Guid id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            
            return View(product);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                var existingProduct = _context.Products.Find(product.Id);
                if (existingProduct == null)
                {
                    return HttpNotFound();
                }
                
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Description = product.Description;
                existingProduct.InStock = product.InStock;
                
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    // Delete old image file if exists
                    if (!string.IsNullOrEmpty(existingProduct.ImagePath))
                    {
                        var oldImagePath = Server.MapPath("~" + existingProduct.ImagePath);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    
                    var fileName = Path.GetFileName(imageFile.FileName);
                    var fileExtension = Path.GetExtension(fileName);
                    var newFileName = Guid.NewGuid().ToString() + fileExtension;
                    var path = Path.Combine(Server.MapPath("~/Content/ProductImages"), newFileName);
                    
                    // Ensure directory exists
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    
                    imageFile.SaveAs(path);
                    existingProduct.ImagePath = "/Content/ProductImages/" + newFileName;
                }
                
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            
            return View(product);
        }

        // GET: Product/Delete/5
        public ActionResult Delete(Guid id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            
            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            
            // Delete image file if exists
            if (!string.IsNullOrEmpty(product.ImagePath))
            {
                var imagePath = Server.MapPath("~" + product.ImagePath);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }
            
            _context.Products.Remove(product);
            _context.SaveChanges();
            
            return RedirectToAction("Index");
        }
    }
} 