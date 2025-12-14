using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shipeazi.Domain.src.ValueObjects
{
    public sealed class Address
    {
        public string Street { get; }
        public string City { get; }
        public string State { get; }
        public string PostalCode { get; }
        public string Country { get; }

        public Address(string street, string city, string state, string postalCode, string country)
        {
            if (string.IsNullOrWhiteSpace(street))
                throw new ArgumentException("Street cannot be null or empty.", nameof(street));

            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City cannot be null or empty.", nameof(city));

            if (string.IsNullOrWhiteSpace(state))
                throw new ArgumentException("State cannot be null or empty.", nameof(state));

            if (string.IsNullOrWhiteSpace(postalCode))
                throw new ArgumentException("Postal code cannot be null or empty.", nameof(postalCode));

            if (string.IsNullOrWhiteSpace(country))
                throw new ArgumentException("Country cannot be null or empty.", nameof(country));

            Street = street;
            City = city;
            State = state;
            PostalCode = postalCode;
            Country = country;
        }

        public override string ToString() => $"{Street}, {City}, {State}, {PostalCode}, {Country}";

        public override bool Equals(object? obj)
        {
            if (obj is Address other)
            {
                return Street == other.Street &&
                       City == other.City &&
                       State == other.State &&
                       PostalCode == other.PostalCode &&
                       Country == other.Country;
            }
            return false;
        }

        public override int GetHashCode() => HashCode.Combine(Street, City, State, PostalCode, Country);
    }
}