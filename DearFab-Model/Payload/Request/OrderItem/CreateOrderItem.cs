namespace DearFab_Model.Payload.Request.OrderItem;

public class CreateOrderItem
{
    public Guid ProductSizeId { get; set; }
    
    public int Quantity { get; set; }
}