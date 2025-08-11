using Microsoft.AspNetCore.Http;

namespace DearFab_Model.Payload.Request.Product;

public class UpdateProductRequest
{
    public string? Name { get; set; }

    public string? Description { get; set; }
    
    public IFormFile? Image { get; set; }
}