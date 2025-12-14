using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shipeazi.Domain.src.Entities;

namespace Shipeazi.Application.src.Repositories
{
    public interface IUserRepository
    {
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByPhoneNumberAsync(string phoneNumber);
    }
}