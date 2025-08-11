using DearFab_Model.Paginate;
using DearFab_Model.Payload.Request.Order;
using DearFab_Model.Payload.Response;
using DearFab_Model.Payload.Response.Order;

namespace DearFab_Service.Interface;

public interface IOrderService
{
    Task<BaseResponse<CreateOrderResponse>> CreateOrder(CreateOrderRequest request);

    Task<BaseResponse<IPaginate<GetOrderResponse>>> GetAllOrder(int page, int size);
    
    Task<BaseResponse<GetOrderDetailResponse>> GetOrderDetail(Guid id);
}