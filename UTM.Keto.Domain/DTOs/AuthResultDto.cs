using System;
using System.Web;

namespace UTM.Keto.Domain.DTOs
{
    public class AuthResultDto
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; }
        public string UserRole { get; set; }
        public HttpCookie Cookie { get; set; }
    }
} 