using DearFab_Model.Payload.Request.Payment;
using DearFab_Model.Payload.Response;
using Net.payOS.Types;

namespace DearFab_Service.Interface;

public interface IPaymentService
{
    Task<BaseResponse<CreatePaymentResult>> CreatePaymentUrlRegisterCreator(CreatePaymentRequest request);
}