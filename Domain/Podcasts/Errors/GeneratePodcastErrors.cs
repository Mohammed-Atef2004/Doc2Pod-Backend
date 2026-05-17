using Domain.SharedKernel;

namespace Domain.Podcasts.Errors
{
    public static class GeneratePodcastErrors
    {
        public static readonly Error FullPodcastAlreadyExists =
        new("Podcast.FullGenerationAlreadyExists", "A full podcast has already been generated for this document.");

        public static readonly Error DocumentNotFound =
            new("Document.NotFound", "The document with the provided ID was not found.");
    }
}
