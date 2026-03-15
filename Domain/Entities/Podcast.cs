using Domain.Enums;
using Domain.SharedKernel;

namespace Domain.Entities
{
    public class Podcast : Entity<Guid>
    {
        public Guid DocumentId { get; private set; }

        public PodcastMode Mode { get; private set; }

        public string? Topic { get; private set; }

        public int? StartPage { get; private set; }

        public int? EndPage { get; private set; }

        public string ScriptPath { get; private set; }

        public Document Document { get; private set; }

        private Podcast() { }

        public Podcast(
            Guid documentId,
            PodcastMode mode,
            string? topic,
            int? startPage,
            int? endPage,
            string scriptPath)
        {
            Id = Guid.NewGuid();
            DocumentId = documentId;
            Mode = mode;
            Topic = topic;
            StartPage = startPage;
            EndPage = endPage;
            ScriptPath = scriptPath;
        }
    }
}