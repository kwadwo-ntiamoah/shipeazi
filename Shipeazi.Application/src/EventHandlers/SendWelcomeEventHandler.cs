using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orchestrix.Mediator;
using Shipeazi.Domain.src.Entities;

namespace Shipeazi.Application.src.EventHandlers
{
    public class UserCreatedNotification(User User): INotification
    {
        public User User { get; } = User;
    }

    public class SendWelcomeEventHandler : INotificationHandler<UserCreatedNotification>
    {
        public ValueTask Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Welcome message sent to user with phone: {notification.User.Phone}");
            return ValueTask.CompletedTask;
        }
    }
}