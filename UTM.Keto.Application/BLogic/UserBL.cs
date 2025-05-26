using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;
using UTM.Keto.Application.Interfaces;
using UTM.Keto.Domain;
using UTM.Keto.Domain.DTOs;
using UTM.Keto.Infrastructure;

namespace UTM.Keto.Application.BLogic
{
    public class UserBL : IUserBL
    {
        private readonly ApplicationDbContext _db;

        public UserBL()
        {
            _db = new ApplicationDbContext();
        }

        public AuthResultDto Authenticate(string email, string password)
        {
            var result = new AuthResultDto
            {
                IsSuccess = false
            };

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                result.ErrorMessage = "Email и пароль обязательны";
                return result;
            }

            var user = _db.Users.AsQueryable().FirstOrDefault(u => u.Email == email);
            var hashedPassword = HashPassword(password);

            if (user != null && user.PasswordHash == hashedPassword)
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
                
                result.IsSuccess = true;
                result.UserId = user.Id.GetHashCode();
                result.Email = user.Email;
                result.UserRole = user.Role.ToString();
                result.Cookie = cookie;
                return result;
            }

            result.ErrorMessage = "Неверный email или пароль";
            return result;
        }

        public AuthResultDto Register(string fullName, string email, string password)
        {
            var result = new AuthResultDto
            {
                IsSuccess = false
            };

            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                result.ErrorMessage = "Все поля обязательны";
                return result;
            }

            var existingUser = _db.Users.AsQueryable().FirstOrDefault(u => u.Email == email);
            if (existingUser != null)
            {
                result.ErrorMessage = "Пользователь с таким email уже существует";
                return result;
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

            // Генерируем аутентификационный билет
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
            
            result.IsSuccess = true;
            result.UserId = user.Id.GetHashCode();
            result.Email = user.Email;
            result.UserRole = user.Role.ToString();
            result.Cookie = cookie;
            return result;
        }

        public SignOutResultDto SignOut()
        {
            FormsAuthentication.SignOut();
            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, "")
            {
                Expires = DateTime.Now.AddYears(-1)
            };
            
            return new SignOutResultDto
            {
                IsSuccess = true,
                Cookie = authCookie
            };
        }

        public User GetUserById(int userId)
        {
            return _db.Users.AsQueryable().FirstOrDefault(u => u.Id.GetHashCode() == userId);
        }

        public User GetUserById(Guid userId)
        {
            return _db.Users.AsQueryable().FirstOrDefault(u => u.Id == userId);
        }

        public User GetUserByEmail(string email)
        {
            return _db.Users.AsQueryable().FirstOrDefault(u => u.Email == email);
        }

        public List<User> GetAllUsers()
        {
            return _db.Users.ToList();
        }

        public bool IsUserInRole(string email, string role)
        {
            var user = GetUserByEmail(email);
            return user != null && user.Role.ToString() == role;
        }

        public string[] GetUserRoles(string email)
        {
            var user = GetUserByEmail(email);
            return user != null ? new[] { user.Role.ToString() } : new string[0];
        }

        public void UpdateUser(User user)
        {
            var existingUser = _db.Users.AsQueryable().FirstOrDefault(u => u.Id == user.Id);
            if (existingUser != null)
            {
                existingUser.FullName = user.FullName;
                existingUser.Email = user.Email;
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.Role = user.Role;
                
                _db.SaveChanges();
            }
        }

        public void DeleteUser(Guid userId)
        {
            var user = _db.Users.AsQueryable().FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                _db.Users.Remove(user);
                _db.SaveChanges();
            }
        }

        public void DeleteUser(int userId)
        {
            var user = _db.Users.AsQueryable().FirstOrDefault(u => u.Id.GetHashCode() == userId);
            if (user != null)
            {
                _db.Users.Remove(user);
                _db.SaveChanges();
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
} 