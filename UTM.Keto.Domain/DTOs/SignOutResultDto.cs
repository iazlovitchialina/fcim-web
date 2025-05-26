using System.Web;

namespace UTM.Keto.Domain.DTOs
{
    public class SignOutResultDto
    {
        public bool IsSuccess { get; set; }
        public HttpCookie Cookie { get; set; }
    }
} 