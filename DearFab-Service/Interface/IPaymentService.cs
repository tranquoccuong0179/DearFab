using DearFab_Model.Payload.Request.Payment;
using DearFab_Model.Payload.Response;
using Net.payOS.Types;
using Transaction = DearFab_Model.Entity.Transaction;

namespace DearFab_Service.Interface;

public interface IPaymentService
{
    Task<BaseResponse<CreatePaymentResult>> CreatePaymentUrlRegisterCreator(CreatePaymentRequest request);

    Task<BaseResponse<bool>> ConfirmWebhook(WebhookType payload);

    Task HandleSuccessPayment(Transaction transaction);

    Task<BaseResponse<bool>> HandleFailedPayment(string paymentLinkId, long orderCode);
}