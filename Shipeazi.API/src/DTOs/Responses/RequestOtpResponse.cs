namespace Shipeazi.API.src.DTOs.Responses
{
    public class RequestOtpResponse
    {
        public string Message { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public Guid OtpId { get; set; }
    }
}
