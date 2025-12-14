using ErrorOr;
using Orchestrix.Mediator;
using Orchestrix.Mediator.Cqrs;
using Shipeazi.Application.src.Repositories;
using Shipeazi.Domain.src.ValueObjects;

namespace Shipeazi.Application.src.Commands
{
    public class CompleteProfileCommand : ICommand<ErrorOr<CompleteProfileResult>>
    {
        public string UserId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }

    public class CompleteProfileResult
    {
        public string UserId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string ShipeaziAddress { get; set; } = string.Empty;
        public bool IsProfileComplete { get; set; }
        public string Message { get; set; } = "Profile completed successfully";
    }

    public class CompleteProfileCommandHandler : ICommandHandler<CompleteProfileCommand, ErrorOr<CompleteProfileResult>>
    {
        private readonly IUserRepository _userRepository;

        public CompleteProfileCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async ValueTask<ErrorOr<CompleteProfileResult>> Handle(CompleteProfileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Get the user by ID
                var user = await _userRepository.GetByIdAsync(Guid.Parse(request.UserId));
                if (user == null)
                {
                    return Error.NotFound(description: "User not found");
                }

                // Check if profile is already complete
                if (user.IsProfileComplete())
                {
                    return Error.Conflict(description: "Profile is already complete");
                }

                // Create address value object
                var address = new Address(
                    street: request.Street,
                    city: request.City,
                    state: request.State,
                    postalCode: request.PostalCode,
                    country: request.Country
                );

                // Complete the profile
                user.CompleteProfile(request.DisplayName, address, request.Email);

                // Update the user in the database
                await _userRepository.UpdateAsync(user);

                return new CompleteProfileResult
                {
                    UserId = user.Id.ToString(),
                    DisplayName = user.DisplayName!,
                    Email = user.Email,
                    ShipeaziAddress = user.ShipeaziAddress,
                    IsProfileComplete = user.IsProfileComplete()
                };
            }
            catch (Exception ex)
            {
                return Error.Failure(description: ex.Message);
            }
        }
    }
}
