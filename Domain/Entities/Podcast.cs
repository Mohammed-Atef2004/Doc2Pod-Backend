using Domain.Enums;
using Domain.SharedKernel;
using Domain.Users;

namespace Domain.Entities
{
    public class Podcast : Entity<Guid>, ISoftDeletable
    {
        public Guid DocumentId { get; private set; }
        public Guid UserId { get; private set; }
        public PodcastMode Mode { get; private set; }

        public string? Topic { get; private set; }

        public int? StartPage { get; private set; }

        public int? EndPage { get; private set; }

        public string ScriptPath { get; private set; }

        public string AudioPath { get; private set; }

        public Document Document { get; private set; }
        public User User { get; private set; }

        private Podcast() { }

        public Podcast(
            Guid documentId,
            Guid userId,
            PodcastMode mode,
            string? topic,
            int? startPage,
            int? endPage,
            string scriptPath,
            string audioPath) : base(Guid.NewGuid())
        {
            UserId = userId;
            DocumentId = documentId;
            Mode = mode;
            Topic = topic;
            StartPage = startPage;
            EndPage = endPage;
            ScriptPath = scriptPath;
            AudioPath = audioPath;
        }
    }
}