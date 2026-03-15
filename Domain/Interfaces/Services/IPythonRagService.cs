namespace Domain.Interfaces.Services
{
    public interface IPythonRagService
    {
        public Task<string> GeneratePodcastAsync(
        string pdfPath,
        int mode,
        string topic,
        int? startPage,
        int? endPage);
    }
}
