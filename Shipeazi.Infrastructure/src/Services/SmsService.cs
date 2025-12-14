using Microsoft.Extensions.Logging;
using Shipeazi.Application.src.Services;

namespace Shipeazi.Infrastructure.src.Services
{
    public class SmsService : ISmsService
    {
        private readonly ILogger<SmsService> _logger;

        public SmsService(ILogger<SmsService> logger)
        {
            _logger = logger;
        }

        public async Task<bool> SendOtpAsync(string phoneNumber, string countryCode, string otpCode)
        {
            // TODO: Integrate with actual SMS provider (Twilio, AWS SNS, Africa's Talking, etc.)
            // For now, just log the OTP (ONLY FOR DEVELOPMENT!)
            
            _logger.LogInformation("========================================");
            _logger.LogInformation("OTP Code for {CountryCode}{PhoneNumber}: {OtpCode}", countryCode, phoneNumber, otpCode);
            _logger.LogInformation("========================================");

            // Simulate sending SMS
            await Task.Delay(100);
            
            return true;
        }
    }
}
