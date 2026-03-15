using Microsoft.AspNetCore.Http;

namespace Domain.Interfaces.Services
{
    public interface IFileStorageService
    {
        public Task<string> SaveFileAsync(IFormFile file);
        public Task<string> SavePodcastScriptAsync(Guid documentId, string script, string fileName);
    }
}
