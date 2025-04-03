using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Security.Cryptography;
using System.Text;
using UTM.Keto.Domain;
using UTM.Keto.Infrastructure;

namespace UTM.Keto.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AuthController()
        {
            _db = new ApplicationDbContext();
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
                ViewBag.Error = "Please enter email and password";
                return View();
            }

            var user = _db.Users.FirstOrDefault(u => u.Email == email);
            var hashedPassword = HashPassword(password);

            if (user != null && user.PasswordHash == hashedPassword)
            {
                AuthenticateUser(user);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid email or password";
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
                ViewBag.Error = "Please fill out all fields";
                return View();
            }

            if (password != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match";
                return View();
            }

            var existingUser = _db.Users.FirstOrDefault(u => u.Email == email);
            if (existingUser != null)
            {
                ViewBag.Error = "User with this email already exists";
                return View();
            }

            // Проверяем, является ли это первым пользователем в системе
            bool isFirstUser = !_db.Users.Any();

            var user = new User
            {
                FullName = fullName,
                Email = email,
                // Если это первый пользователь, даем ему роль Admin
                Role = isFirstUser ? UserRole.Admin : UserRole.Guest,
                PasswordHash = HashPassword(password)
            };

            _db.Users.Add(user);
            _db.SaveChanges();

            AuthenticateUser(user);
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, "")
            {
                Expires = DateTime.Now.AddYears(-1)
            };
            HttpContext.Response.Cookies.Add(authCookie);

            return RedirectToAction("Index", "Home");
        }

        private void AuthenticateUser(User user)
        {
            var userData = $"{user.Id}|{user.Email}|{user.Role}";
            
            var ticket = new FormsAuthenticationTicket(
                1,
                user.Email,
                DateTime.Now,
                DateTime.Now.AddHours(12),
                true, 
                userData, 
                FormsAuthentication.FormsCookiePath);

            var encryptedTicket = FormsAuthentication.Encrypt(ticket);
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
            {
                HttpOnly = true,
                Secure = FormsAuthentication.RequireSSL,
                Path = FormsAuthentication.FormsCookiePath
            };
            
            HttpContext.Response.Cookies.Add(cookie);
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
} 