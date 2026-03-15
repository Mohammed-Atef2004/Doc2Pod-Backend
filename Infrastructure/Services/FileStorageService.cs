using Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.IO;

namespace Infrastructure.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public FileStorageService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<string> SaveFileAsync(IFormFile file)
        {
            var url = _config["Supabase:Url"];
            var key = _config["Supabase:Key"];
            var bucket = "PDFs";

            var extension = Path.GetExtension(file.FileName);
            var safeFileName = $"{Guid.NewGuid()}{extension}";

            var requestUrl = $"{url}/storage/v1/object/{bucket}/{safeFileName}";

            using var stream = file.OpenReadStream();
            var content = new StreamContent(stream);

            content.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);
            if (!_httpClient.DefaultRequestHeaders.Contains("apikey"))
            {
                _httpClient.DefaultRequestHeaders.Add("apikey", key);
            }

            content.Headers.Add("x-upsert", "true");

            var response = await _httpClient.PostAsync(requestUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorDetails = await response.Content.ReadAsStringAsync();
                throw new Exception($"Supabase Storage Error (SaveFile): {errorDetails}");
            }

            return $"{url}/storage/v1/object/public/{bucket}/{safeFileName}";
        }

        public async Task<string> SavePodcastScriptAsync(Guid documentId, string script, string fileName)
        {
            var url = _config["Supabase:Url"];
            var key = _config["Supabase:Key"];
            var bucket = "Podcasts";

          
            var safeFileName = fileName.Replace(" ", "_");
            var filePath = $"{documentId}/{safeFileName}";
            var requestUrl = $"{url}/storage/v1/object/{bucket}/{filePath}";

          
            var cleanScript = script
                .Replace("\\n", Environment.NewLine) 
                .Replace("\\\"", "\"")               
                .Trim('"');                          

        
            var content = new StringContent(cleanScript, System.Text.Encoding.UTF8, "text/plain");

       

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);
            if (!_httpClient.DefaultRequestHeaders.Contains("apikey"))
            {
                _httpClient.DefaultRequestHeaders.Add("apikey", key);
            }

            content.Headers.Add("x-upsert", "true");

            var response = await _httpClient.PostAsync(requestUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorDetails = await response.Content.ReadAsStringAsync();
                throw new Exception($"Supabase Storage Error (SaveScript): {errorDetails}");
            }

            return $"{url}/storage/v1/object/public/{bucket}/{filePath}";
        }
    }
}