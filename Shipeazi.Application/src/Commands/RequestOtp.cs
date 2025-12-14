using ErrorOr;
using Orchestrix.Mediator;
using Orchestrix.Mediator.Cqrs;
using Shipeazi.Application.src.Repositories;
using Shipeazi.Application.src.Services;
using Shipeazi.Domain.src.Entities;

namespace Shipeazi.Application.src.Commands
{
    public class RequestOtpCommand(string phoneNumber, string countryCode, string purpose) : ICommand<ErrorOr<RequestOtpResult>>
    {
        public string PhoneNumber { get; set; } = phoneNumber;
        public string CountryCode { get; set; } = countryCode;
        public string Purpose { get; set; } = purpose; // "registration", "login", "password_reset"
    }

    public class RequestOtpResult
    {
        public string Message { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public Guid OtpId { get; set; }
    }

    public class RequestOtpCommandHandler(
        IOtpVerificationRepository otpRepository,
        IUserRepository userRepository,
        ISmsService smsService) : ICommandHandler<RequestOtpCommand, ErrorOr<RequestOtpResult>>
    {
        public async ValueTask<ErrorOr<RequestOtpResult>> Handle(RequestOtpCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate purpose
                if (request.Purpose != "registration" && request.Purpose != "login")
                {
                    return Error.Validation(description: "Invalid OTP purpose. Must be 'registration' or 'login'");
                }

                // For registration, check if user already exists
                if (request.Purpose == "registration")
                {
                    var existingUser = await userRepository.GetByPhoneNumberAsync($"{request.CountryCode}{request.PhoneNumber}");
                    if (existingUser != null)
                    {
                        return Error.Conflict(description: "Phone number already registered. Use 'login' instead.");
                    }
                }

                // For login, check if user exists
                if (request.Purpose == "login")
                {
                    var existingUser = await userRepository.GetByPhoneNumberAsync($"{request.CountryCode}{request.PhoneNumber}");
                    if (existingUser == null)
                    {
                        return Error.NotFound(description: "Phone number not registered. Please register first.");
                    }
                }

                // Generate 6-digit OTP
                var random = new Random();
                var otpCode = random.Next(100000, 999999).ToString();

                // Create OTP record
                var otp = new OtpVerification(
                    phoneNumber: request.PhoneNumber,
                    countryCode: request.CountryCode,
                    otpCode: otpCode,
                    purpose: request.Purpose
                );

                await otpRepository.AddAsync(otp);

                // Send OTP via SMS
                var smsSent = await smsService.SendOtpAsync(request.PhoneNumber, request.CountryCode, otpCode);
                
                if (!smsSent)
                {
                    return Error.Failure(description: "Failed to send OTP. Please try again.");
                }

                return new RequestOtpResult
                {
                    Message = $"OTP sent to {request.CountryCode}{request.PhoneNumber}",
                    ExpiresAt = otp.ExpiresAt,
                    OtpId = otp.Id
                };
            }
            catch (Exception ex)
            {
                return Error.Failure(description: ex.Message);
            }
        }
    }
}
