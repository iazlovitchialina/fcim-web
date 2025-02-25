using System.Web.Mvc;

namespace UTM.Keto.Web.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            ViewBag.Title = "Home";
            return View();
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
    }
}