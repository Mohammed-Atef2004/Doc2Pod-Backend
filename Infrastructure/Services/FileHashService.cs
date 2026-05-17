using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;


namespace Infrastructure.Services
{
    public class FileHashService : IFileHashService
    {
        public async Task<string> GenerateHashAsync(IFormFile file)
        {
            using var sha256 = SHA256.Create();
            using var stream = file.OpenReadStream();
            var hashBytes =
                await sha256.ComputeHashAsync(stream);

            return Convert.ToHexString(hashBytes);
        }
    }
}
