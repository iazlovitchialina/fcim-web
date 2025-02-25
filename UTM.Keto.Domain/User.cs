using System;
using System.Collections.Generic;
using UTM.Keto.Domain;

public class User
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public UserRole Role { get; set; }
    public List<Booking> Bookings { get; set; }
}

public enum UserRole
{
    Guest,
    Admin
}