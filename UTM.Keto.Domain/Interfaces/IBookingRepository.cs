using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UTM.Keto.Domain;

namespace UTM.Keto.Domain.Interfaces
{
    public interface IBookingRepository : IRepository<Booking>
    {
        Task<IReadOnlyList<Booking>> GetBookingsByUserIdAsync(Guid userId);
        Task<IReadOnlyList<Booking>> GetBookingsByRoomIdAsync(Guid roomId);
        Task<IReadOnlyList<Booking>> GetBookingsByStatusAsync(BookingStatus status);
        Task<IReadOnlyList<Booking>> GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<bool> IsRoomAvailableAsync(Guid roomId, DateTime checkIn, DateTime checkOut);
    }
} 