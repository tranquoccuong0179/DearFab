using DearFab_Model.Payload.Request.Payment;
using DearFab_Model.Payload.Response;
using DearFab_Service.Interface;
using DearFab.Constant;
using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using Newtonsoft.Json;

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
    
    [HttpPost(ApiEndPointConstant.Payment.HandlePayment)]
    public async Task<IActionResult> HandleWebhook([FromBody] WebhookType payload)
    {
        try
        {
            var signatureFromPayOs = payload.signature;
            var requestBody = JsonConvert.SerializeObject(payload);
            var result = await _paymentService.ConfirmWebhook(payload);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while handling webhook in controller.");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing the webhook.");
        }
    }
    
    [HttpGet(ApiEndPointConstant.Payment.ReturnUrlFail)]
    public async Task<IActionResult> ReturnFailedUrl()
    {
        string responseCode = Request.Query["code"].ToString();
        string id = Request.Query["id"].ToString();
        string cancel = Request.Query["cancel"].ToString();
        string status = Request.Query["status"].ToString();
        string orderCode = Request.Query["orderCode"];

        if (status == "CANCELLED")
        {
            var response = await _paymentService.HandleFailedPayment(id, long.Parse(orderCode));
            return Redirect("https://www.dearfab.com/order-cancle");
        }
        return Redirect("https://www.dearfab.com/order-cancle");
    }
}