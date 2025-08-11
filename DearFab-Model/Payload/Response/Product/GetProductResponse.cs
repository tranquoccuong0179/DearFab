namespace DearFab_Model.Payload.Response.Product;

public class GetProductResponse
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Image { get; set; }
    
    public double? Price { get; set; }
    
    public int? Quantity { get; set; }
}