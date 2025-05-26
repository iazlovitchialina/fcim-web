using System;

namespace UTM.Keto.Domain.DTOs
{
    public class UserProfileDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime? RegistrationDate { get; set; }
    }
} 