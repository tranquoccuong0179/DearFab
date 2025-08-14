namespace DearFab_Model.Payload.Response.Review;

public class GetReviewResponse
{
    public Guid Id { get; set; }
    
    public double Rating { get; set; }
    
    public string? Content { get; set; }
    
    public string? FullName { get; set; }
    
    public DateTime? CreateAt { get; set; }
}