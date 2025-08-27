using DearFab_Model.Payload.Response.OrderItem;

namespace DearFab_Model.Payload.Response.Order;

public class GetOrderDetailResponse
{
    public Guid Id { get; set; }

    public string? FullName { get; set; }
    
    public double TotalPrice { get; set; }

    public string? Address { get; set; }
    
    public string? Phone { get; set; }

    public string Status { get; set; } = null!;
    
    public List<GetOrderItemResponse>? items { get; set; }
}