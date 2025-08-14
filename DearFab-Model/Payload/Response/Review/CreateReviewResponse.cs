namespace DearFab_Model.Payload.Response.Review;

public class CreateReviewResponse
{
    public Guid ProductId { get; set; }
    
    public double Rating { get; set; }

    public string Content { get; set; } = null!;
    
    public Guid AccountId { get; set; }
}