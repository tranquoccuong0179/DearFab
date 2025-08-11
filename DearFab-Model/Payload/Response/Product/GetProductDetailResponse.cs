using DearFab_Model.Payload.Response.ProductSize;

namespace DearFab_Model.Payload.Response.Product;

public class GetProductDetailResponse
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }
    
    public string? Image { get; set; }

    public List<GetProductSizeResponse> productSizes { get; set; }
}