using Microsoft.AspNetCore.Http;
using Shipeazi.Application.src.Services;

namespace Shipeazi.Infrastructure.src.Services
{
    public class HttpContextService(IHttpContextAccessor httpContextAccessor) : IHttpContextService
    {
        public string GetClientIpAddress()
        {
            var context = httpContextAccessor.HttpContext;
            if (context == null) return "unknown";

            // Check for forwarded IP first (if behind a proxy)
            var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                return forwardedFor.Split(',')[0].Trim();
            }

            // Fall back to remote IP
            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
    }
}
