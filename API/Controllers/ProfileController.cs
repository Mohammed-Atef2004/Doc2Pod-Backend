using Application.Features.Users.Commands.Profile;
using Application.Features.Users.Commands.Profile.ChangeEmail;
using Application.Features.Users.Commands.Profile.ChangePassword;
using Application.Features.Users.Commands.Profile.SetPhoneNumber;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProfileController(IMediator mediator)
    {
        _mediator = mediator;
    }

   
    [HttpPut("change-name")]
    public async Task<IActionResult> ChangeName([FromBody] ChangeNameCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPut("change-email")]
    public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

  
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }


   
    [HttpPut("set-phone-number")]
    public async Task<IActionResult> SetPhoneNumber([FromBody] SetPhoneNumberCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}