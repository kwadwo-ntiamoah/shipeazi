using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ErrorOr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Shipeazi.API.src.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult ToProblem(List<Error> errors)
        {
            if (errors.Count == 0)
            {
                return Problem();
            }

            var firstError = errors[0];
            var statusCode = firstError.Type switch
            {
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                ErrorType.Forbidden => StatusCodes.Status403Forbidden,
                _ => StatusCodes.Status500InternalServerError
            };

            return Problem(
                statusCode: statusCode,
                title: firstError.Code,
                detail: string.Join("; ", errors.Select(e => e.Description)),
                extensions: errors.Count > 1
                    ? new Dictionary<string, object?> { ["errors"] = errors.Select(e => new { e.Code, e.Description }) }
                    : null
            );
        }

        protected string? GetAuthenticatedUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        protected IActionResult UnauthorizedUserNotFound()
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "Unauthorized",
                Detail = "User ID not found in token",
                Status = StatusCodes.Status401Unauthorized
            });
        }
    }
}