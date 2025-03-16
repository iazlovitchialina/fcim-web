using System;
using System.Threading.Tasks;
using UTM.Keto.Domain.Interfaces;
using UTM.Keto.Infrastructure.Repositories;

namespace UTM.Keto.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IUserRepository Users { get; }
        public IHotelRepository Hotels { get; }
        public IBookingRepository Bookings { get; }
        public IPaymentRepository Payments { get; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Users = new UserRepository(context);
            Hotels = new HotelRepository(context);
            Bookings = new BookingRepository(context);
            Payments = new PaymentRepository(context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}