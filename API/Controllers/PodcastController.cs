using Application.Features.Podcasts.Commands.GeneratePodcast;
using Application.Features.Podcasts.Query.GetAllPodcasts;
using Application.Features.Podcasts.Query.GetPodcast;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/podcast")]
    public class PodcastController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PodcastController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost("generate")]
        [Authorize]
        public async Task<IActionResult> Generate(GeneratePodcastCommand command)
        {
            var userIdString = User.FindFirstValue("domain_user_id");

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized(new { message = "User ID not found in token" });
            }

            var commandWithUser = command with { UserId = userId };
            var result = await _mediator.Send(commandWithUser);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPodcasts(CancellationToken cancellationToken)
        {
            var userIdString = User.FindFirstValue("domain_user_id");

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized(new { message = "User identity not found in token." });
            }

            var query = new GetAllPodcastsQuery(userId);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAudio(Guid id)
        {
            var stream = await _mediator.Send(new GetPodcastQuery(id));
            return File(stream, "audio/mpeg",
                enableRangeProcessing: true
            );

        }
    }
}