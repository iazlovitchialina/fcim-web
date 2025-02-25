using System;
using System.Collections.Generic;

namespace UTM.Keto.Domain
{
    public class Hotel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public List<Room> Rooms { get; set; } 
    }
}