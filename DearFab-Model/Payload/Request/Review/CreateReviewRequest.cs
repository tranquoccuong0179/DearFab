namespace DearFab_Model.Payload.Request.Review;

public class CreateReviewRequest
{
    public double Rating { get; set; }

    public string Content { get; set; } = null!;
}