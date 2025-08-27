using DearFab_Model.Payload.Response;
using DearFab_Service.Interface;
using DearFab.Constant;
using Microsoft.AspNetCore.Mvc;

namespace DearFab.Controller;

public class ReviewController : BaseController<ReviewController>
{
    private readonly IReviewService _reviewService;

    public ReviewController(ILogger<ReviewController> logger, IReviewService reviewService) : base(logger)
    {
        _reviewService = reviewService;
    }
    
    [HttpDelete(ApiEndPointConstant.Review.DeleteReview)]
    [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status404NotFound)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> DeleteReview([FromRoute] Guid id)
    {
        var response = await _reviewService.DeleteReview(id);
        return StatusCode(response.Status, response);
    }
}