using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UTM.Keto.Domain;

namespace UTM.Keto.Domain.Interfaces
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        Task<IReadOnlyList<Payment>> GetPaymentsByBookingIdAsync(Guid bookingId);
        Task<IReadOnlyList<Payment>> GetPaymentsByStatusAsync(PaymentStatus status);
        Task<decimal> GetTotalPaymentsForBookingAsync(Guid bookingId);
    }
} 