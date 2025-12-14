using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shipeazi.Domain.src.Common;
using Shipeazi.Domain.src.Events;
using Shipeazi.Domain.src.ValueObjects;

namespace Shipeazi.Domain.src.Entities
{
    public class User: BaseEntity
    {
        public string? Email { get; private set; }
        public string? DisplayName { get; private set; } 
        public PhoneNumber Phone { get; private set; }
        public Address? Address { get; private set; }
        public string ShipeaziAddress { get; private set; } = string.Empty;

        // Constructor for new user registration
        public User(PhoneNumber phone)
        {
            Phone = phone ?? throw new ArgumentNullException(nameof(phone));
            
            // Add domain event
            AddDomainEvent(new UserCreatedEvent(this));
        }

        // Private constructor for AutoMapper and database loading (no domain events)
        private User(Guid id, PhoneNumber phone, string? email, string? displayName, Address? address, string shipeaziAddress)
        {
            Id = id;
            Phone = phone ?? throw new ArgumentNullException(nameof(phone));
            Email = email;
            DisplayName = displayName;
            Address = address;
            ShipeaziAddress = shipeaziAddress;
        }

        // Factory method for loading existing user from database
        public static User Load(Guid id, PhoneNumber phone, string? email, string? displayName, Address? address, string shipeaziAddress)
        {
            return new User(id, phone, email, displayName, address, shipeaziAddress);
        }

        public bool IsProfileComplete()
        {
            return !string.IsNullOrWhiteSpace(DisplayName) 
                   && Address != null 
                   && !string.IsNullOrWhiteSpace(ShipeaziAddress);
        }

        public void CompleteProfile(string displayName, Address address, string? email = null)
        {
            if (string.IsNullOrWhiteSpace(displayName))
                throw new ArgumentException("Display name cannot be null or empty.", nameof(displayName));

            DisplayName = displayName;
            Address = address ?? throw new ArgumentNullException(nameof(address));
            Email = email;
            
            // Generate Shipeazi address when profile is complete
            GenerateShipeaziAddress();
        }

        private void GenerateShipeaziAddress()
        {
            if (Address == null)
                throw new InvalidOperationException("Cannot generate Shipeazi address without a complete profile.");
            
            // shipeazi address should be in format "SHP-{First3LettersOfCity}-{PhoneNumber}"
            var cityCode = Address.City.Length >= 3 
                ? Address.City[..3].ToUpper() 
                : Address.City.ToUpper();
            
            ShipeaziAddress = $"SHP-{cityCode}-{Phone.Value}"; //eg: SHP-ACC-0540609437
        }
    }
}