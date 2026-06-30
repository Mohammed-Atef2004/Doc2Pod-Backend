using Domain.Documents;
using Domain.Enums;
using Domain.Podcasts.Errors;
using Domain.Podcasts.Events;
using Domain.SharedKernel;
using Domain.Users;


namespace Domain.Podcasts
{
    public class Podcast : AggregateRoot<Guid>, ISoftDeletable
    {
        public Guid DocumentId { get; private set; }
        public Guid UserId { get; private set; }
        public PodcastMode Mode { get; private set; }
        public string? Topic { get; private set; }
        public string TTSModel { get; private set; }
        public int? StartPage { get; private set; }

        public int? EndPage { get; private set; }

        public string? ScriptPath { get; private set; }

        public string? AudioPath { get; private set; }

        public PodcastStatus Status { get; private set; } = PodcastStatus.Pending;

        public string? ErrorMessage { get; private set; }
        public Document Document { get; private set; }
        public User User { get; private set; }
        private Podcast() { }

        public Podcast(
            Guid userId,
            Guid documentId,
            string ttsModel,
            PodcastMode mode,
            string? topic,
            int? startPage,
            int? endPage,
            PodcastStatus podcastStatus)

        {
            Id = Guid.NewGuid();
            UserId = userId;
            DocumentId = documentId;
            TTSModel = ttsModel;
            Mode = mode;
            Topic = topic;
            StartPage = startPage;
            EndPage = endPage;
            Status = podcastStatus;

        }

        public Result SetPaths(string scriptPath, string audioPath)
        {
            if (string.IsNullOrWhiteSpace(scriptPath))
                return Result<Podcast>.Failure(GeneratePodcastErrors.GenerationFailed);

            if (string.IsNullOrWhiteSpace(audioPath))
                return Result<Podcast>.Failure(GeneratePodcastErrors.GenerationFailed);

            ScriptPath = scriptPath;
            AudioPath = audioPath;
            return Result.Success();
        }

        public Result UpdateStatus(PodcastStatus status, string? error = null)
        {
            Status = status;
            if (status == PodcastStatus.Completed)
            {
                ErrorMessage = null;
            }
            else
            {
                ErrorMessage = error;
            }
            AddDomainEvent(new PodcastStatusChangedDomainEvent(UserId, Id, status.ToString()));
            return Result.Success();
        }
    }
}
