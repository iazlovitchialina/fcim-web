using System;
using System.Linq;
using System.Web.Mvc;
using UTM.Keto.Application;
using UTM.Keto.Application.Interfaces;
using UTM.Keto.Domain;

namespace UTM.Keto.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IProductBL _productBL;

        public ProductController()
        {
            var factory = BusinessLogicFactory.Instance;
            _productBL = factory.GetProductBL();
        }

        public ActionResult Index()
        {
            var products = _productBL.GetAllProducts();
            return View(products);
        }

        public ActionResult Details(int id)
        {
            var product = _productBL.GetProductById(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _productBL.AddProduct(product);
                return RedirectToAction("Index");
            }
            return View(product);
        }

        public ActionResult Edit(int id)
        {
            var product = _productBL.GetProductById(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                _productBL.UpdateProduct(product);
                return RedirectToAction("Index");
            }
            return View(product);
        }

        public ActionResult Delete(int id)
        {
            var product = _productBL.GetProductById(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _productBL.DeleteProduct(id);
            return RedirectToAction("Index");
        }
    }
} 