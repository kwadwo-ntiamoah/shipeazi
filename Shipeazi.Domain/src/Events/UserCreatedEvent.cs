using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shipeazi.Domain.src.Common;
using Shipeazi.Domain.src.Entities;

namespace Shipeazi.Domain.src.Events
{
    public class UserCreatedEvent(User user) : BaseDomainEvent
    {
        public Guid UserId { get; } = user.Id;
        public string Email { get; } = user.Email;
        public string DisplayName { get; } = user.DisplayName;
        public string ShipeaziAddress { get; } = user.ShipeaziAddress ?? string.Empty;
    }
}