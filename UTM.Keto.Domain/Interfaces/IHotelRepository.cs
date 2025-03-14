using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UTM.Keto.Domain;

namespace UTM.Keto.Domain.Interfaces
{
    public interface IHotelRepository : IRepository<Hotel>
    {
        Task<IReadOnlyList<Hotel>> GetHotelsByCityAsync(string city);
        Task<IReadOnlyList<Room>> GetHotelRoomsAsync(Guid hotelId);
        Task<IReadOnlyList<Room>> GetAvailableRoomsAsync(Guid hotelId, DateTime checkIn, DateTime checkOut);
    }
} 