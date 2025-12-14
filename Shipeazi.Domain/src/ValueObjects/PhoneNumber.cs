using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shipeazi.Domain.src.ValueObjects
{
    public sealed class PhoneNumber
    {
        public string CountryCode { get; }
        public string Value { get; }

        public PhoneNumber(string countryCode, string value)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
                throw new ArgumentException("Country code cannot be null or empty.", nameof(countryCode));

            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Phone number value cannot be null or empty.", nameof(value));

            CountryCode = countryCode;
            Value = value;
        }

        public override string ToString() => $"+{CountryCode} {Value}";
        
        public override bool Equals(object? obj)
        {
            if (obj is PhoneNumber other)
            {
                return CountryCode == other.CountryCode && Value == other.Value;
            }
            return false;
        }

        public override int GetHashCode() => HashCode.Combine(CountryCode, Value);
    }
}