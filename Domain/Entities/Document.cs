using Domain.Enums;
using Domain.SharedKernel;

namespace Domain.Entities
{
    public class Document : Entity<Guid>, ISoftDeletable
    {
        public string FileName { get; private set; }
        public string FilePath { get; private set; }


        private readonly List<Podcast> _podcasts = new List<Podcast>();
        public IReadOnlyCollection<Podcast> Podcasts => _podcasts.AsReadOnly();

        protected Document() { }

        public Document(string fileName, string filePath) : base(Guid.NewGuid())
        {
            FileName = fileName;
            FilePath = filePath;
        }

        public Podcast AddPodcast(Guid UserId, PodcastMode mode, string? topic, int? startPage, int? endPage, string scriptPath, string audioPath)
        {

            var podcast = new Podcast(
                Id,
                UserId,
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