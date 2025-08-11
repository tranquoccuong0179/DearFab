namespace DearFab_Model.Payload.Response.Order;

public class CreateOrderResponse
{
    public List<Guid>? ProductSizeId { get; set; }
    
    public double TotalPrice { get; set; }
    
    public string? Address { get; set; }
    
    public string Status { get; set; } = null!;
}