using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using UTM.Keto.Domain;
using UTM.Keto.Domain.Interfaces;

namespace UTM.Keto.Infrastructure.Repositories
{
    public class BookingRepository : Repository<Booking>, IBookingRepository
    {
        public BookingRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Booking>> GetBookingsByUserIdAsync(Guid userId)
        {
            return await _context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.Room)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Booking>> GetBookingsByRoomIdAsync(Guid roomId)
        {
            return await _context.Bookings
                .Where(b => b.RoomId == roomId)
                .Include(b => b.User)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Booking>> GetBookingsByStatusAsync(BookingStatus status)
        {
            return await _context.Bookings
                .Where(b => b.Status == status)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Booking>> GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Bookings
                .Where(b => 
                    (b.CheckInDate >= startDate && b.CheckInDate <= endDate) || 
                    (b.CheckOutDate >= startDate && b.CheckOutDate <= endDate) ||
                    (b.CheckInDate <= startDate && b.CheckOutDate >= endDate)) 
                .Include(b => b.Room)
                .Include(b => b.User)
                .ToListAsync();
        }

        public async Task<bool> IsRoomAvailableAsync(Guid roomId, DateTime checkIn, DateTime checkOut)
        {
            var overlappingBookings = await _context.Bookings
                .Where(b => b.RoomId == roomId &&
                           ((b.CheckInDate < checkOut && b.CheckOutDate > checkIn) || 
                            (b.CheckInDate >= checkIn && b.CheckInDate < checkOut) || 
                            (b.CheckOutDate > checkIn && b.CheckOutDate <= checkOut)))
                .ToListAsync();

            return !overlappingBookings.Any();
        }
    }
}