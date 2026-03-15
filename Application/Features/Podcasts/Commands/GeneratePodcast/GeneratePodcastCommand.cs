using Domain.Enums;
using MediatR;

namespace Application.Features.Podcasts.Commands.GeneratePodcast
{
    public record GeneratePodcastCommand(Guid DocumentId, PodcastMode Mode, string? Topic, int? StartPage, int? EndPage) : IRequest<Guid>;

}
