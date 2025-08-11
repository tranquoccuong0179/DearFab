using DearFab_Model.Payload.Request.OrderItem;

namespace DearFab_Model.Payload.Request.Order;

public class CreateOrderRequest
{
    public List<CreateOrderItem> OrderItems { get; set; } = new List<CreateOrderItem>();
    public string Address { get; set; } = null!;
}