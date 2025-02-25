﻿using System;

namespace UTM.Keto.Domain
{
    public class Booking
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
    
        public Guid RoomId { get; set; }
        public Room Room { get; set; }
    
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public BookingStatus Status { get; set; }
    }

    public enum BookingStatus
    {
        Pending,
        Confirmed,
        Canceled
    }
}