using System.ComponentModel.DataAnnotations;

namespace Shipeazi.API.src.DTOs.Requests
{
    public class RequestOtpRequest
    {
        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Country code is required")]
        [RegularExpression(@"^\+\d{1,4}$", ErrorMessage = "Country code must start with + and contain 1-4 digits")]
        public string CountryCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Purpose is required")]
        [RegularExpression(@"^(registration|login|password_reset)$", ErrorMessage = "Purpose must be 'registration', 'login', or 'password_reset'")]
        public string Purpose { get; set; } = string.Empty;
    }
}
