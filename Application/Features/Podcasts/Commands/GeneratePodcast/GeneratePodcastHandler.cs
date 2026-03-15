using Application.Common.Exceptions;
using Domain.Enums; 
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Entities;
using MediatR;

namespace Application.Features.Podcasts.Commands.GeneratePodcast
{
    public class GeneratePodcastHandler : IRequestHandler<GeneratePodcastCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorageService;
        private readonly IPythonRagService _pythonRagService;

        public GeneratePodcastHandler(
            IUnitOfWork unitOfWork,
            IFileStorageService fileStorageService,
            IPythonRagService pythonRagService)
        {
            _unitOfWork = unitOfWork;
            _fileStorageService = fileStorageService;
            _pythonRagService = pythonRagService;
        }

        public async Task<Guid> Handle(GeneratePodcastCommand request, CancellationToken cancellationToken)
        {
            var document = await _unitOfWork.Document.GetByIdAsync(request.DocumentId);

            if (document == null)
                throw new NotFoundException(nameof(Document), request.DocumentId);

            var script = await _pythonRagService.GeneratePodcastAsync(
                document.FilePath,
                (int)request.Mode,
                request.Topic,
                request.StartPage,
                request.EndPage
            );

            var uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8);
            var fileName = $"podcast_mode{(int)request.Mode}_{uniqueId}.txt";

            var scriptPath = await _fileStorageService.SavePodcastScriptAsync(
                request.DocumentId,
                script,
                fileName
            );


            var podcast = document.AddPodcast(
                (PodcastMode)request.Mode,
                request.Topic,
                request.StartPage,
                request.EndPage,
                scriptPath
            );

     
            await _unitOfWork.Podcast.AddAsync(podcast);

            await _unitOfWork.CompleteAsync(cancellationToken);

            return podcast.Id;
        }
    }
}