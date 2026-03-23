using Domain.Enums;
using Domain.SharedKernel;

namespace Domain.Entities
{
    public class Document : Entity<Guid>, ISoftDeletable
    {
        public string FileName { get; private set; }
        public string FilePath { get; private set; }
        public DateTime UploadedAt { get; private set; }

        private readonly List<Podcast> _podcasts = new List<Podcast>();
        public IReadOnlyCollection<Podcast> Podcasts => _podcasts.AsReadOnly();

        protected Document() { }

        public Document(string fileName, string filePath)
        {
            FileName = fileName;
            FilePath = filePath;
            UploadedAt = DateTime.UtcNow;
        }

        public Podcast AddPodcast(PodcastMode mode, string? topic, int? startPage, int? endPage, string scriptPath, string audioPath)
        {

            if (_podcasts.Any(p => p.Mode == mode))
                throw new Exception("Podcast already exists for this mode");

            var podcast = new Podcast(
                Id,
                mode,
                topic,
                startPage,
                endPage,
                scriptPath,
                audioPath
            );

            _podcasts.Add(podcast);

            return podcast;
        }
    }
}