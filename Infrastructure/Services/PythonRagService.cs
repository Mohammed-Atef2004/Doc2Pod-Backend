using Domain.Interfaces.Services;
using System.Net.Http.Headers;
using System.IO;

namespace Infrastructure.Services
{
    public class PythonRagService : IPythonRagService
    {
        private readonly HttpClient _httpClient;

        public PythonRagService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GeneratePodcastAsync(
            string pdfPath,
            int mode,
            string? topic,
            int? startPage,
            int? endPage)
        {
            using var form = new MultipartFormDataContent();

            using var downloadClient = new HttpClient();
            var fileBytes = await downloadClient.GetByteArrayAsync(pdfPath);
            var fileContent = new ByteArrayContent(fileBytes);

            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            var fileName = Path.GetFileName(new Uri(pdfPath).LocalPath);

            form.Add(fileContent, "file", fileName);
            form.Add(new StringContent(mode.ToString()), "mode");

            if (!string.IsNullOrEmpty(topic))
            {
                form.Add(new StringContent(topic), "topic");
            }
            if (startPage.HasValue)
            {
                form.Add(new StringContent(startPage.Value.ToString()), "start_page");
            }
            if (endPage.HasValue)
            {
                form.Add(new StringContent(endPage.Value.ToString()), "end_page");
            }

            var response = await _httpClient.PostAsync("generate", form);

            if (!response.IsSuccessStatusCode)
            {
                var errorDetails = await response.Content.ReadAsStringAsync();
                throw new Exception($"Python service failed: {response.StatusCode} - {errorDetails}");
            }

            var rawResponse = await response.Content.ReadAsStringAsync();

            var parts = rawResponse.Split("---SCRIPT_START---");

            var script = parts.Length > 1 ? parts[1] : rawResponse;

            return script.Trim();
        }
    }
}