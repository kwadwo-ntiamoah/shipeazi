using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shipeazi.Application.src.Repositories;
using Shipeazi.Domain.src.Entities;
using Shipeazi.Infrastructure.src.Identity;

namespace Shipeazi.Infrastructure.src.Persistence.Repositories
{
    public class UserRepository(UserManager<AppUser> userManager, IMapper mapper) : IUserRepository
    {
        public async Task AddAsync(User user)
        {
            // Generate username from phone number
            var username = $"{user.Phone}";

            var appUser = new AppUser
            {
                Id = user.Id.ToString(),
                UserName = username,
                PhoneNumber = username
            };

            // Create user without password (passwordless authentication via OTP)
            var result = await userManager.CreateAsync(appUser);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to create user: {errors}");
            }
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            var appUser = await userManager.FindByIdAsync(id.ToString());
            if (appUser == null) return null;

            var user = mapper.Map<User>(appUser);
            return user;
        }

        public async Task<User?> GetByPhoneNumberAsync(string phoneNumber)
        {
            var appUser = await userManager.Users
                .FirstOrDefaultAsync(u => u.UserName == phoneNumber);
            if (appUser == null) return null;

            var user = mapper.Map<User>(appUser);
            return user;
        }
    }
}