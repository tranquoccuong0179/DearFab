using System.Text.Json;
using Appwrite;
using DearFab_Model.Payload.Settings;
using DearFab_Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client.Extensions.Msal;

namespace DearFab_Service.Implement;

public class UploadService :  IUploadService
    {
        private readonly AppWriteSettings _appWriteSettings;
        private readonly HttpClient _httpClient;

        public UploadService(AppWriteSettings appWriteSettings, HttpClient httpClient)
        {
            _appWriteSettings = appWriteSettings;
            _httpClient = httpClient;
        }

        public async Task<string> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No file uploaded.");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp"};
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (string.IsNullOrEmpty(fileExtension) || !allowedExtensions.Contains(fileExtension))
                throw new ArgumentException("Only image files are allowed (.jpg, .jpeg, .png, .gif, .bmp, .webp).");

            if (string.IsNullOrEmpty(file.FileName))
                throw new ArgumentException("File name cannot be empty.");
            if (string.IsNullOrEmpty(file.ContentType))
                throw new ArgumentException("File content type cannot be empty.");

            try
            {
                using var formContent = new MultipartFormDataContent();
                formContent.Add(new StringContent("unique()"), "fileId");

                using var fileStream = file.OpenReadStream();
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                formContent.Add(fileContent, "file", file.FileName);

                var request = new HttpRequestMessage(HttpMethod.Post,
                    $"{_appWriteSettings.EndPoint}/storage/buckets/{_appWriteSettings.Bucket}/files");

                request.Headers.Add("X-Appwrite-Project", _appWriteSettings.ProjectId);
                request.Headers.Add("X-Appwrite-Key", _appWriteSettings.APIKey);
                request.Content = formContent;

                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    using var jsonDoc = JsonDocument.Parse(responseContent);
                    var fileId = jsonDoc.RootElement.GetProperty("$id").GetString();

                    var fileUrl = $"{_appWriteSettings.EndPoint}/storage/buckets/{_appWriteSettings.Bucket}/files/{fileId}/view?project={_appWriteSettings.ProjectId}";
                    return fileUrl;
                }
                else
                {
                    throw new Exception($"Upload failed with status {response.StatusCode}: {responseContent}");
                }
            }
            catch (AppwriteException ex)
            {
                throw new Exception($"Upload failed: {ex.Message}");
            }
        }
    }