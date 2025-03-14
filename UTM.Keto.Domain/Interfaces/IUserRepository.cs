using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UTM.Keto.Domain;

namespace UTM.Keto.Domain.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<IReadOnlyList<User>> GetUsersByRoleAsync(UserRole role);
        Task<IReadOnlyList<Booking>> GetUserBookingsAsync(Guid userId);
    }
} 