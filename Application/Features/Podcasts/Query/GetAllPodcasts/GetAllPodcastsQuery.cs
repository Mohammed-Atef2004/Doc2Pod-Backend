using MediatR;

namespace Application.Features.Podcasts.Query.GetAllPodcasts
{
    public record GetAllPodcastsQuery(Guid UserId) : IRequest<List<GetAllPodcastsResponse>>;

}
