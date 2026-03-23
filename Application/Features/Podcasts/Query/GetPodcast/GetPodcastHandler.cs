using Application.Interfaces;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.Features.Podcasts.Query.GetPodcast
{
    public class GetPodcastHandler
    {
        public class GetAudioStreamQueryHandler : IRequestHandler<GetPodcastQuery, Stream>
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

            public async Task<Stream> Handle(GetPodcastQuery request, CancellationToken cancellationToken)
            {
                var audio = await _unitOfWork.Podcast
                    .GetByIdAsync(request.Id);

                if (audio == null)
                    throw new Exception("Audio not found");

                if (string.IsNullOrEmpty(audio.AudioPath))
                    throw new Exception("File path is missing");


                var signedUrl = await _storageService
                      .GetSignedUrlAsync("Podcasts", audio.AudioPath);

                var response = await _httpClient.GetAsync(
                    signedUrl,
                    HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken);

                if (!response.IsSuccessStatusCode)
                    throw new Exception("Failed to fetch audio");

                return await response.Content.ReadAsStreamAsync(cancellationToken);
            }
        }
    }
}
