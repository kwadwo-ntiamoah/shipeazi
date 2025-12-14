using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shipeazi.Domain.src.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; } = Guid.NewGuid();
        private readonly List<BaseDomainEvent> _domainEvents = [];
        public IReadOnlyCollection<BaseDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected void AddDomainEvent(BaseDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}