using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orchestrix.Mediator;
using Shipeazi.Domain.src.Entities;
using Shipeazi.Domain.src.Events;

namespace Shipeazi.Application.src.EventHandlers
{
    public class UserCreatedNotification(User user): UserCreatedEvent(user), INotification {}

    public class SendWelcomeEventHandler : INotificationHandler<UserCreatedNotification>
    {
        public ValueTask Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Welcome email with address {notification.ShipeaziAddress} sent to {notification.Email}");
            return ValueTask.CompletedTask;
        }
    }
}