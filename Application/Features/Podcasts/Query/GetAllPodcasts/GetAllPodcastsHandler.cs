using AutoMapper;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.Features.Podcasts.Query.GetAllPodcasts
{
    public class GetAllPodcastsHandler : IRequestHandler<GetAllPodcastsQuery, List<GetAllPodcastsResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetAllPodcastsHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<GetAllPodcastsResponse>> Handle(GetAllPodcastsQuery request, CancellationToken cancellationToken)
        {
            var podcasts = await _unitOfWork.Podcast.GetByUserId(request.UserId);
            return _mapper.Map<List<GetAllPodcastsResponse>>(podcasts);

        }
    }
}
