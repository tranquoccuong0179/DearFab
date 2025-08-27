using DearFab_Model.Paginate;
using DearFab_Model.Payload.Request.Order;
using DearFab_Model.Payload.Response;
using DearFab_Model.Payload.Response.Order;
using DearFab_Service.Interface;
using DearFab.Constant;
using Microsoft.AspNetCore.Mvc;

namespace DearFab.Controller;

public class OrderController : BaseController<OrderController>
{
    private readonly IOrderService _orderService;

    public OrderController(ILogger<OrderController> logger, IOrderService orderService) : base(logger)
    {
        _orderService = orderService;
    }
    
    [HttpPost(ApiEndPointConstant.Order.CreateOrder)]
    [ProducesResponseType(typeof(BaseResponse<CreateOrderResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<CreateOrderResponse>), StatusCodes.Status500InternalServerError)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var response = await _orderService.CreateOrder(request);
        return StatusCode(response.Status, response);
    }
    
    [HttpGet(ApiEndPointConstant.Order.GetOrders)]
    [ProducesResponseType(typeof(BaseResponse<IPaginate<GetOrderResponse>>), StatusCodes.Status200OK)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> GetAccounts([FromQuery] int? page, [FromQuery] int? size)
    {
        int pageNumber = page ?? 1;
        int pageSize = size ?? 10;
        var response = await _orderService.GetAllOrder(pageNumber, pageSize);
        return StatusCode(response.Status, response);
    }
    
    [HttpGet(ApiEndPointConstant.Order.GetOrder)]
    [ProducesResponseType(typeof(BaseResponse<GetOrderDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<GetOrderDetailResponse>), StatusCodes.Status404NotFound)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> GetAccounts([FromRoute] Guid id)
    {
        var response = await _orderService.GetOrderDetail(id);
        return StatusCode(response.Status, response);
    }
    
    [HttpPut(ApiEndPointConstant.Order.ChangeStatus)]
    [ProducesResponseType(typeof(BaseResponse<GetOrderResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<GetOrderResponse>), StatusCodes.Status404NotFound)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> ChangeStatus([FromRoute] Guid id)
    {
        var response = await _orderService.ChangeStatus(id);
        return StatusCode(response.Status, response);
    }
}