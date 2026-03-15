using Domain.SharedKernel;
using Domain.Enums;

namespace Domain.Entities
{
    public class Document : Entity<Guid>
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

        public Podcast AddPodcast(PodcastMode mode, string? topic, int? startPage, int? endPage, string scriptPath)
        {
            var podcast = new Podcast(
                this.Id,
                mode,
                topic,
                startPage,
                endPage,
                scriptPath
            );

            _podcasts.Add(podcast);

            return podcast;
        }
    }
}