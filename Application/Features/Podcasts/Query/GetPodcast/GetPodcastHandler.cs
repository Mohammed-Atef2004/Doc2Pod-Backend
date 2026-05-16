using Application.Interfaces;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Domain.Podcasts.Errors;
using Domain.SharedKernel;
using MediatR;

namespace Application.Features.Podcasts.Query.GetPodcast
{
    public class GetPodcastHandler
    {
        public class GetAudioStreamQueryHandler : IRequestHandler<GetPodcastQuery, Result<Stream>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IFileStorageService _storageService;
            private readonly HttpClient _httpClient;

            public GetAudioStreamQueryHandler(
                IUnitOfWork unitOfWork,
                IFileStorageService storageService,
                HttpClient httpClient)
            {
                _unitOfWork = unitOfWork;
                _storageService = storageService;
                _httpClient = httpClient;
            }

            public async Task<Result<Stream>> Handle(GetPodcastQuery request, CancellationToken cancellationToken)
            {
                var podcast = await _unitOfWork.Podcast.GetByIdAsync(request.Id);

                if (podcast == null)
                    return Result<Stream>.Failure(GetPodcastErrors.NotFound);

                if (podcast.Status != PodcastStatus.Completed)
                    return Result<Stream>.Failure(GetPodcastErrors.NotReady);

                if (string.IsNullOrEmpty(podcast.AudioPath))
                    return Result<Stream>.Failure(GetPodcastErrors.MissingAudioPath);

                var signedUrl = await _storageService
                      .GetSignedUrlAsync("Podcasts", podcast.AudioPath);

                var response = await _httpClient.GetAsync(
                    signedUrl,
                    HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken);

                if (!response.IsSuccessStatusCode)
                    return Result<Stream>.Failure(GetPodcastErrors.StorageAccessFailed);

                var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

                return Result<Stream>.Success(stream);
            }
        }
    }
}
