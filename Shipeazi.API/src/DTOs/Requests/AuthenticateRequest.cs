using System.ComponentModel.DataAnnotations;

namespace Shipeazi.API.src.DTOs.Requests
{
    public class AuthenticateRequest
    {
        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Country code is required")]
        [RegularExpression(@"^\+\d{1,4}$", ErrorMessage = "Country code must start with + and contain 1-4 digits")]
        public string CountryCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "OTP code is required")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "OTP must be 6 digits")]
        public string OtpCode { get; set; } = string.Empty;
    }
}
