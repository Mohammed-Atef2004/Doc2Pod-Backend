using Application.Interfaces;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Domain.Podcasts.Errors;
using Domain.SharedKernel;
using Domain.Users.Errors;
using Hangfire;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Features.Podcasts.Commands.GeneratePodcast
{
    public class GeneratePodcastHandler : IRequestHandler<GeneratePodcastCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPythonRagService _ragService;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IUserContext _userContext;

        public GeneratePodcastHandler(
            IPythonRagService ragService,
            IUnitOfWork unitOfWork,
            IServiceScopeFactory scopeFactory,
            IUserContext userContext)
        {
            _ragService = ragService;
            _unitOfWork = unitOfWork;
            _scopeFactory = scopeFactory;
            _userContext = userContext;
        }

        public async Task<Result<Guid>> Handle(GeneratePodcastCommand command, CancellationToken cancellationToken)
        {
            var userid = _userContext.UserId;
            if (userid == null)
            {
                return Result<Guid>.Failure(UserErrors.InvalidUserId);

            }

            var initialDocument = await _unitOfWork.Document.GetByIdAsync(command.DocumentId);

            if (initialDocument == null)
                return Result<Guid>.Failure(GeneratePodcastErrors.DocumentNotFound);

            if (command.Mode == PodcastMode.Full)
            {
                var alreadyExists = await _unitOfWork.Podcast.ExistsAsync(p =>
                    p.DocumentId == command.DocumentId &&
                    (int)p.Mode == 3);

                if (alreadyExists)
                {
                    return Result<Guid>.Failure(GeneratePodcastErrors.FullPodcastAlreadyExists);
                }
            }

            var podcast = initialDocument.AddPodcast
            (
                userid.Value,
                command.Mode,
                command.Topic,
                command.StartPage,
                command.EndPage,
                PodcastStatus.Pending
            );

            await _unitOfWork.Podcast.AddAsync(podcast);
            await _unitOfWork.CompleteAsync(cancellationToken);

            BackgroundJob.Enqueue<IPodcastService>(s =>
                s.ProcessPodcastGenerationAsync(podcast.Id, initialDocument.FilePath, command));

            return Result<Guid>.Success(podcast.Id);

        }
    }
}
