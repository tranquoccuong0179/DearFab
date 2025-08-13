using DearFab_Model.Paginate;
using DearFab_Model.Payload.Request.Product;
using DearFab_Model.Payload.Request.ProductSize;
using DearFab_Model.Payload.Request.Review;
using DearFab_Model.Payload.Response;
using DearFab_Model.Payload.Response.Product;
using DearFab_Model.Payload.Response.Review;
using DearFab_Service.Interface;
using DearFab.Constant;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DearFab.Controller;

public class ProductController : BaseController<ProductController>
{
    private readonly IProductService _productService;
    private readonly IReviewService _reviewService;
    public ProductController(ILogger<ProductController> logger, IProductService productService, IReviewService reviewService) : base(logger)
    {
        _productService = productService;
        _reviewService = reviewService;
    }
    
    [HttpPost(ApiEndPointConstant.Product.CreateProduct)]
    [ProducesResponseType(typeof(BaseResponse<CreateProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<CreateProductResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BaseResponse<CreateProductResponse>), StatusCodes.Status500InternalServerError)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> CreateProduct([FromForm] CreateProductRequest request)
    {
        var response = await _productService.CreateProduct(request);
        return StatusCode(response.Status, response);
    }
    
    [HttpGet(ApiEndPointConstant.Product.GetAllProduct)]
    [ProducesResponseType(typeof(BaseResponse<IPaginate<GetProductResponse>>), StatusCodes.Status200OK)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> GetAllProduct([FromQuery] int? page, [FromQuery] int? size)
    {
        int pageNumber = page ?? 1;
        int pageSize = size ?? 10;
        var response = await _productService.GetAllProduct(pageNumber, pageSize);
        return StatusCode(response.Status, response);
    }
    
    [HttpGet(ApiEndPointConstant.Product.GetProductById)]
    [ProducesResponseType(typeof(BaseResponse<GetProductDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<GetProductDetailResponse>), StatusCodes.Status404NotFound)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> GetProductById([FromRoute] Guid id)
    {
        var response = await _productService.GetProductById(id);
        return StatusCode(response.Status, response);
    }
    
    [HttpPut(ApiEndPointConstant.Product.UpdateProduct)]
    [ProducesResponseType(typeof(BaseResponse<UpdateProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<UpdateProductResponse>), StatusCodes.Status404NotFound)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> UpdateProduct([FromRoute] Guid id, [FromForm] UpdateProductRequest request)
    {
        var response = await _productService.UpdateProduct(id, request);
        return StatusCode(response.Status, response);
    }
    
    [HttpDelete(ApiEndPointConstant.Product.DeleteProduct)]
    [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status404NotFound)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> DeleteProduct([FromRoute] Guid id)
    {
        var response = await _productService.DeleteProduct(id);
        return StatusCode(response.Status, response);
    }
    
    [HttpPost(ApiEndPointConstant.Product.CreateReview)]
    [ProducesResponseType(typeof(BaseResponse<CreateReviewResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<CreateReviewResponse>), StatusCodes.Status404NotFound)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> CreateReview([FromRoute] Guid id, [FromBody] CreateReviewRequest request)
    {
        var response = await _reviewService.CreateReview(id, request);
        return StatusCode(response.Status, response);
    }
    
    [HttpGet(ApiEndPointConstant.Product.GetAllReview)]
    [ProducesResponseType(typeof(BaseResponse<IPaginate<GetReviewResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<IPaginate<GetReviewResponse>>), StatusCodes.Status404NotFound)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> GetAllReview([FromRoute] Guid id, [FromQuery] int? page, [FromQuery] int? size)
    {
        int pageNumber = page ?? 1;
        int pageSize = size ?? 10;
        var response = await _reviewService.GetAllReviews(id, pageNumber, pageSize);
        return StatusCode(response.Status, response);
    }
}