namespace Shipeazi.API.src.DTOs.Responses
{
    public class CompleteProfileResponse
    {
        public string UserId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string ShipeaziAddress { get; set; } = string.Empty;
        public bool IsProfileComplete { get; set; }
        public string Message { get; set; } = "Profile completed successfully";
    }
}
