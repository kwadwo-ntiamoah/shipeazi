using Microsoft.AspNetCore.Identity;
using Shipeazi.Domain.src.ValueObjects;

namespace Shipeazi.Infrastructure.src.Identity
{
    public class AppUser: IdentityUser
    {
        public string DisplayName {get; set;} = string.Empty;
        public string ShipeaziAddress {get; set;} = string.Empty;
        public PhoneNumber Phone {get; set;} = null!;
        public Address? Address {get; set;} // Nullable - populated when profile is complete
    }
}