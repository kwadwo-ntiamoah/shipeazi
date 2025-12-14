using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shipeazi.Domain.src.Common;
using Shipeazi.Domain.src.ValueObjects;

namespace Shipeazi.Domain.src.Entities
{
    public class User: BaseEntity
    {
        public string Phone { get; private set; }

        // Constructor for new user registration
        public User(string phone)
        {
            Phone = phone ?? throw new ArgumentNullException(nameof(phone));
        }

        // Private constructor for AutoMapper and database loading (no domain events)
        private User(Guid id, string phone)
        {
            Id = id;
            Phone = phone ?? throw new ArgumentNullException(nameof(phone));
        }

        // Factory method for loading existing user from database
        public static User Load(Guid id, string phone)
        {
            return new User(id, phone);
        }
    }
}