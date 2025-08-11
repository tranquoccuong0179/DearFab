using DearFab_Model.Payload.Request.ProductSize;
using DearFab_Model.Payload.Response;
using DearFab_Model.Payload.Response.ProductSize;

namespace DearFab_Service.Interface;

public interface IProductSizeService
{
    Task<BaseResponse<bool>> DeleteProductSize(Guid id);
    
    Task<BaseResponse<GetProductSizeResponse>> UpdateProductSize(Guid id, UpdateProductSizeRequest request); 
}