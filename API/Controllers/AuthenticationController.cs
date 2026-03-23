using Application.Features.Users.Commands.Authentication.ConfirmEmail;
using Application.Features.Users.Commands.Authentication.ConfirmEmailChange;
using Application.Features.Users.Commands.Authentication.ForgotPassword;
using Application.Features.Users.Commands.Authentication.Login;
using Application.Features.Users.Commands.Authentication.Logout;
using Application.Features.Users.Commands.Authentication.Register;
using Application.Features.Users.Commands.Authentication.ResetPassword;
using Application.Features.Users.Commands.Authentication.VerifyTwoFactor;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthenticationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ─────────────────────────────────────────────
        // Authentication
        // ─────────────────────────────────────────────


        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }


        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }


        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] LogoutCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        //API Design Issue 
        [HttpGet("confirm-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromQuery] Guid userId, [FromQuery] string token, CancellationToken ct)
        {
            byte[] decodedBytes = WebEncoders.Base64UrlDecode(token);
            string originalToken = Encoding.UTF8.GetString(decodedBytes);

            var command = new ConfirmEmailCommand(userId, originalToken);
            var result = await _mediator.Send(command, ct);

            return result.IsSuccess
                ? Ok("Email confirmed successfully! You can now login.")
                : BadRequest(result.Error);
        }

        [HttpPost("confirm-email-change")]
        [Authorize]
        public async Task<IActionResult> ConfirmEmailChange([FromBody] ConfirmEmailChangeCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }


        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }


        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpPost("verify-two-factor")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyTwoFactor([FromBody] VerifyTwoFactorCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

    }
}
