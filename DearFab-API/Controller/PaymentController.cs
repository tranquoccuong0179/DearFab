using DearFab_Model.Payload.Request.Payment;
using DearFab_Model.Payload.Response;
using DearFab_Service.Interface;
using DearFab.Constant;
using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;

namespace DearFab.Controller;

public class PaymentController : BaseController<PaymentController>
{
    private readonly IPaymentService _paymentService;

    public PaymentController(ILogger<PaymentController> logger, IPaymentService paymentService) : base(logger)
    {
        _paymentService = paymentService;
    }
    
    [HttpPost(ApiEndPointConstant.Payment.CreatePayment)]
    [ProducesResponseType(typeof(BaseResponse<CreatePaymentResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<CreatePaymentResult>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BaseResponse<CreatePaymentResult>), StatusCodes.Status500InternalServerError)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
    {
        var response = await _paymentService.CreatePaymentUrlRegisterCreator(request);
        return StatusCode(response.Status, response);
    }
}