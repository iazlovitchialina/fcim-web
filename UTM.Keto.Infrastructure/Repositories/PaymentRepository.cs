using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using UTM.Keto.Domain;
using UTM.Keto.Domain.Interfaces;

namespace UTM.Keto.Infrastructure.Repositories
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        public PaymentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Payment>> GetPaymentsByBookingIdAsync(Guid bookingId)
        {
            return await _context.Payments
                .Where(p => p.BookingId == bookingId)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Payment>> GetPaymentsByStatusAsync(PaymentStatus status)
        {
            return await _context.Payments
                .Where(p => p.Status == status)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalPaymentsForBookingAsync(Guid bookingId)
        {
            return await _context.Payments
                .Where(p => p.BookingId == bookingId && p.Status == PaymentStatus.Completed)
                .SumAsync(p => p.Amount);
        }
    }
}