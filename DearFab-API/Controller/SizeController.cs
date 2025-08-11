using DearFab_Model.Paginate;
using DearFab_Model.Payload.Request.Size;
using DearFab_Model.Payload.Response;
using DearFab_Model.Payload.Response.Size;
using DearFab_Service.Interface;
using DearFab.Constant;
using Microsoft.AspNetCore.Mvc;

namespace DearFab.Controller;

public class SizeController : BaseController<SizeController>
{
    private readonly ISizeService _sizeService;

    public SizeController(ILogger<SizeController> logger, ISizeService sizeService) : base(logger)
    {
        _sizeService = sizeService;
    }
    
    [HttpPost(ApiEndPointConstant.Size.CreateSize)]
    [ProducesResponseType(typeof(BaseResponse<CreateSizeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<CreateSizeResponse>), StatusCodes.Status500InternalServerError)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> CreateSize([FromBody] CreateSizeRequest request)
    {
        var response = await _sizeService.CreateSize(request);
        return StatusCode(response.Status, response);
    }
    
    [HttpGet(ApiEndPointConstant.Size.GetSizes)]
    [ProducesResponseType(typeof(BaseResponse<IPaginate<GetSizeResponse>>), StatusCodes.Status200OK)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> GetSizes([FromQuery] int? page, [FromQuery] int? size)
    {
        int pageNumber = page ?? 1;
        int pageSize = size ?? 10;
        
        var response = await _sizeService.GetSizes(pageNumber, pageSize);
        return StatusCode(response.Status, response);
    }
    
    [HttpGet(ApiEndPointConstant.Size.GetSize)]
    [ProducesResponseType(typeof(BaseResponse<GetSizeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<GetSizeResponse>), StatusCodes.Status404NotFound)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> GetSize([FromRoute] Guid id)
    {
        var response = await _sizeService.GetSize(id);
        return StatusCode(response.Status, response);
    }
}