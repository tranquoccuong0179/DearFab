namespace DearFab_Model.Payload.Request.ProductSize;

public class CreateProductSizeRequest
{
    public Guid SizeId { get; set; }
    
    public double Price { get; set; }
    
    public int Quantity { get; set; }
}