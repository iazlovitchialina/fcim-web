using System.Web.Mvc;
using UTM.Keto.Application;
using UTM.Keto.Application.Interfaces;

namespace UTM.Keto.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductBL _productBL;

        public HomeController()
        {
            var factory = BusinessLogicFactory.Instance;
            _productBL = factory.GetProductBL();
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            ViewBag.Title = "Home";
            var featuredProducts = _productBL.GetFeaturedProducts();
            return View(featuredProducts);
        }

        [AllowAnonymous]
        public ActionResult About()
        {
            ViewBag.Title = "About Us";
            ViewBag.Message = "Your application description page.";
            return View();
        }

        [AllowAnonymous]
        public ActionResult Room()
        {
            ViewBag.Title = "Our Rooms";
            return View();
        }

        [AllowAnonymous]
        public ActionResult Gallery()
        {
            ViewBag.Title = "Gallery";
            return View();
        }

        [AllowAnonymous]
        public ActionResult Blog()
        {
            ViewBag.Title = "Blog";
            return View();
        }

        [AllowAnonymous]
        public ActionResult Contact()
        {
            ViewBag.Title = "Contact Us";
            ViewBag.Message = "Your contact page.";
            return View();
        }

        [AllowAnonymous]
        public ActionResult Error()
        {
            ViewBag.Title = "Error";
            return View("Error");
        }

        public ActionResult AccessDenied()
        {
            Response.StatusCode = 403;
            return View();
        }
    }
}