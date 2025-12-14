using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orchestrix.Mediator;
using Shipeazi.API.src.DTOs.Requests;
using Shipeazi.API.src.DTOs.Responses;
using Shipeazi.Application.src.Commands;

namespace Shipeazi.API.src.Controllers.v1
{
    public class AuthController(IMediator mediator, IMapper mapper) : BaseController
    {
        [AllowAnonymous]
        [HttpPost("otp"), MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(RequestOtpResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RequestOtp([FromBody] RequestOtpRequest request, CancellationToken cancellationToken)
        {
            var command = mapper.Map<RequestOtpCommand>(request);
            var result = await mediator.Send(command, cancellationToken);

            return result.Match(
                success => Ok(mapper.Map<RequestOtpResponse>(result.Value)),
                errors => ToProblem(errors)
            );
        }

        [AllowAnonymous]
        [HttpPost, MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(AuthenticateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest request, CancellationToken cancellationToken)
        {
            var command = mapper.Map<AuthenticateCommand>(request);
            var result = await mediator.Send(command, cancellationToken);

            return result.Match(
                success => Ok(mapper.Map<AuthenticateResponse>(result.Value)),
                errors => ToProblem(errors)
            );
        }

        [AllowAnonymous]
        [HttpPost("refresh"), MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(RefreshTokenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var command = mapper.Map<RefreshTokenCommand>(request);
            var result = await mediator.Send(command, cancellationToken);

            return result.Match(
                success => Ok(mapper.Map<RefreshTokenResponse>(result.Value)),
                errors => ToProblem(errors)
            );
        }

        [Authorize]
        [HttpPost("profile/complete"), MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(CompleteProfileResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CompleteProfile([FromBody] CompleteProfileRequest request, CancellationToken cancellationToken)
        {
            var userId = GetAuthenticatedUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return UnauthorizedUserNotFound();
            }

            var command = mapper.Map<CompleteProfileCommand>(request);
            command.UserId = userId;

            var result = await mediator.Send(command, cancellationToken);

            return result.Match(
                success => Ok(mapper.Map<CompleteProfileResponse>(result.Value)),
                errors => ToProblem(errors)
            );
        }
    }
}
