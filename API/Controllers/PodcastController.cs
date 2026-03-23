using Application.Features.Podcasts.Commands.GeneratePodcast;
using Application.Features.Podcasts.Query.GetPodcast;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Generate([FromBody] GeneratePodcastCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error occurred while generating podcast",
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
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