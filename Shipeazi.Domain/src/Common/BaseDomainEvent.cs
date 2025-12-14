using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shipeazi.Domain.src.Common
{
    public abstract class BaseDomainEvent
    {
        public DateTime OccurredOn { get; protected set; } = DateTime.UtcNow;
    }
}