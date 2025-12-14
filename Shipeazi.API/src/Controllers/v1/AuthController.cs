using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orchestrix.Mediator;
using Shipeazi.API.src.DTOs.Requests;
using Shipeazi.API.src.DTOs.Responses;
using Shipeazi.Application.src.Commands;
using Microsoft.AspNetCore.Http;

namespace Shipeazi.API.src.Controllers.v1
{
    public class AuthController(IMediator mediator, IMapper mapper) : BaseController
    {
        [AllowAnonymous]
        [HttpPost("otp"), MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(RequestOtpResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [EndpointSummary("Request OTP for authentication")]
        [EndpointDescription("Sends a one-time password (OTP) to the provided email or phone number for authentication purposes. The OTP will be valid for a limited time and can be used to authenticate the user.")]
        [Tags("Authentication")]
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
        [EndpointSummary("Authenticate user with OTP")]
        [EndpointDescription("Authenticates a user by verifying the OTP code sent to their email or phone number. Returns access and refresh tokens upon successful authentication.")]
        [Tags("Authentication")]
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
        [EndpointSummary("Refresh access token")]
        [EndpointDescription("Generates a new access token using a valid refresh token. This allows users to maintain their session without re-authenticating when their access token expires.")]
        [Tags("Authentication")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var command = mapper.Map<RefreshTokenCommand>(request);
            var result = await mediator.Send(command, cancellationToken);

            return result.Match(
                success => Ok(mapper.Map<RefreshTokenResponse>(result.Value)),
                errors => ToProblem(errors)
            );
        }
    }
}
