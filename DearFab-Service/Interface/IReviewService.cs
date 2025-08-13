using DearFab_Model.Paginate;
using DearFab_Model.Payload.Request.Review;
using DearFab_Model.Payload.Response;
using DearFab_Model.Payload.Response.Review;

namespace DearFab_Service.Interface;

public interface IReviewService
{
    Task<BaseResponse<CreateReviewResponse>> CreateReview(Guid id, CreateReviewRequest createReviewRequest);
    
    Task<BaseResponse<IPaginate<GetReviewResponse>>> GetAllReviews(Guid id, int page, int size);
}