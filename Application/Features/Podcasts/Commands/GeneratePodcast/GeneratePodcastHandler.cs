using Application.Common.Exceptions;
using Application.Features.Podcasts.DTOs.Requests;
using Application.Features.Podcasts.DTOs.Responses;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Features.Podcasts.Commands.GeneratePodcast
{
    public class GeneratePodcastHandler : IRequestHandler<GeneratePodcastCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPythonRagService _ragService;
        private readonly IServiceScopeFactory _scopeFactory;

        public GeneratePodcastHandler(
            IPythonRagService ragService,
            IUnitOfWork unitOfWork,
            IServiceScopeFactory scopeFactory)
        {
            _ragService = ragService;
            _unitOfWork = unitOfWork;
            _scopeFactory = scopeFactory;
        }

        public async Task<Guid> Handle(GeneratePodcastCommand command, CancellationToken cancellationToken)
        {

            var initialDocument = await _unitOfWork.Document.GetByIdAsync(command.DocumentId);

            if (initialDocument == null)
                throw new NotFoundException(nameof(Document), command.DocumentId);

            var request = new GeneratePodcastRequest
            {
                FileKey = initialDocument.FilePath,
                Mode = (int)command.Mode,
                Topic = command.Topic,
                StartPage = command.StartPage,
                EndPage = command.EndPage
            };


            var taskId = await _ragService.StartGenerationAsync(request);
            PodcastGenerationStatusResponse status = null;

            int maxRetries = 800;

            while (maxRetries > 0)
            {
                maxRetries--;
                await Task.Delay(TimeSpan.FromSeconds(15), cancellationToken);

                try
                {
                    status = await _ragService.GetStatusAsync(taskId);

                    if (status.Status == "DONE")
                    {
                        break;
                    }
                    else if (status.Status == "ERROR")
                    {
                        throw new Exception($"AI Error: {status.Error}");
                    }
                }
                catch (HttpRequestException)
                {
                    Console.WriteLine("Network blip from Cloudflare ignored. Retrying...");
                    continue;
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine("Request timeout ignored. Retrying...");
                    continue;
                }
            }

            if (status == null || status.Status != "DONE")
                throw new Exception("Podcast generation timeout");

            if (string.IsNullOrEmpty(status.AudioPath))
                throw new Exception("Audio path is NULL");

            if (string.IsNullOrEmpty(status.ScriptPath))
                throw new Exception("Script path is NULL");


            using (var scope = _scopeFactory.CreateScope())
            {
                var freshUnitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var freshDocument = await freshUnitOfWork.Document.GetByIdAsync(command.DocumentId);

                if (freshDocument == null)
                    throw new NotFoundException(nameof(Document), command.DocumentId);

                var podcast = freshDocument.AddPodcast(
                    command.UserId,
                    command.Mode,
                    command.Topic,
                    command.StartPage,
                    command.EndPage,
                    status.ScriptPath,
                    status.AudioPath
                );

                await freshUnitOfWork.Podcast.AddAsync(podcast);
                await freshUnitOfWork.CompleteAsync(cancellationToken);

                return podcast.Id;
            }
        }
    }
}
