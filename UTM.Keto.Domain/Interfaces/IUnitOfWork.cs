using System;
using System.Threading.Tasks;

namespace UTM.Keto.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IHotelRepository Hotels { get; }
        IBookingRepository Bookings { get; }
        IPaymentRepository Payments { get; }
        
        Task<int> CompleteAsync();
    }
} 