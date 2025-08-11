using Microsoft.AspNetCore.Http;

namespace DearFab_Service.Interface;

public interface IUploadService
{
    Task<string> UploadImage(IFormFile file);
}