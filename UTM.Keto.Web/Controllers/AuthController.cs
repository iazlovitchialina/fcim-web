using System;
using System.Web;
using System.Web.Mvc;
using UTM.Keto.Application;
using UTM.Keto.Application.Interfaces;
using UTM.Keto.Domain.DTOs;

namespace UTM.Keto.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserBL _userBL;

        public AuthController()
        {
            var factory = BusinessLogicFactory.Instance;
            _userBL = factory.GetUserBL();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Введите email и пароль";
                return View();
            }

            var result = _userBL.Authenticate(email, password);
            if (result.IsSuccess)
            {
                HttpContext.Response.Cookies.Add(result.Cookie);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = result.ErrorMessage;
            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string fullName, string email, string password, string confirmPassword)
        {
            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) || 
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                ViewBag.Error = "Заполните все поля";
                return View();
            }

            if (password != confirmPassword)
            {
                ViewBag.Error = "Пароли не совпадают";
                return View();
            }

            var result = _userBL.Register(fullName, email, password);
            if (result.IsSuccess)
            {
                HttpContext.Response.Cookies.Add(result.Cookie);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = result.ErrorMessage;
            return View();
        }

        public ActionResult Logout()
        {
            var result = _userBL.SignOut();
            HttpContext.Response.Cookies.Add(result.Cookie);
            return RedirectToAction("Index", "Home");
        }
    }
} 