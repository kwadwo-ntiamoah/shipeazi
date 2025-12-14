namespace Shipeazi.Application.src.Services
{
    public interface ISmsService
    {
        Task<bool> SendOtpAsync(string phoneNumber, string countryCode, string otpCode);
    }
}
