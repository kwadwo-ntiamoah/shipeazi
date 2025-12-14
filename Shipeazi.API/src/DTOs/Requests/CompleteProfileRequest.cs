using System.ComponentModel.DataAnnotations;

namespace Shipeazi.API.src.DTOs.Requests
{
    public class CompleteProfileRequest
    {
        [Required(ErrorMessage = "Display name is required")]
        [MinLength(2, ErrorMessage = "Display name must be at least 2 characters")]
        [MaxLength(100, ErrorMessage = "Display name cannot exceed 100 characters")]
        public string DisplayName { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Street is required")]
        public string Street { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "State is required")]
        public string State { get; set; } = string.Empty;

        [Required(ErrorMessage = "Postal code is required")]
        public string PostalCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Country is required")]
        public string Country { get; set; } = string.Empty;
    }
}
