namespace DearFab_Model.Payload.Response.ProductSize;

public class GetProductSizeResponse
{
    public Guid Id { get; set; }
    
    public double? Price { get; set; }
    
    public int? Quantity { get; set; }
    
    public string? Size { get; set; }
}