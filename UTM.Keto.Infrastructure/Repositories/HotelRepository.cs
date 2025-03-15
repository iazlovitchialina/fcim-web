using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using UTM.Keto.Domain;
using UTM.Keto.Domain.Interfaces;

namespace UTM.Keto.Infrastructure.Repositories
{
    public class HotelRepository : Repository<Hotel>, IHotelRepository
    {
        public HotelRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Hotel>> GetHotelsByCityAsync(string city)
        {
            return await _context.Hotels.Where(h => h.City == city).ToListAsync();
        }

        public async Task<IReadOnlyList<Room>> GetHotelRoomsAsync(Guid hotelId)
        {
            return await _context.Rooms.Where(r => r.HotelId == hotelId).ToListAsync();
        }

        public async Task<IReadOnlyList<Room>> GetAvailableRoomsAsync(Guid hotelId, DateTime checkIn, DateTime checkOut)
        {
            return await _context.Rooms
                .Where(r => r.HotelId == hotelId && r.IsAvailable)
                .ToListAsync();
        }
    }
}