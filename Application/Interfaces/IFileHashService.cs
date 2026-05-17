using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface IFileHashService
    {
        public Task<string> GenerateHashAsync(IFormFile file);
    }
}
