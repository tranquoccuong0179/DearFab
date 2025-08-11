using DearFab_Model.Payload.Request.ProductSize;
using DearFab_Model.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DearFab_Model.Payload.Request.Product;

public class CreateProductRequest
{
    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public IFormFile Image { get; set; } = null!;
    
    [ModelBinder(BinderType = typeof(JsonModelBinder))]
    public List<CreateProductSizeRequest> Sizes { get; set; } = null!;
}