using System;

namespace UTM.Keto.Domain
{
    public class Room
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public RoomType Type { get; set; }
        public decimal PricePerNight { get; set; }
        public bool IsAvailable { get; set; }
        public Guid HotelId { get; set; }
        public Hotel Hotel { get; set; }
    }

    public enum RoomType
    {
        Single,
        Double,
        Suite
    }
}