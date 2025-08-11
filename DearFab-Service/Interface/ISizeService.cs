using DearFab_Model.Paginate;
using DearFab_Model.Payload.Request.Size;
using DearFab_Model.Payload.Response;
using DearFab_Model.Payload.Response.Size;

namespace DearFab_Service.Interface;

public interface ISizeService
{
    Task<BaseResponse<CreateSizeResponse>> CreateSize(CreateSizeRequest request);

    Task<BaseResponse<IPaginate<GetSizeResponse>>> GetSizes(int page, int size);
    
    Task<BaseResponse<GetSizeResponse>> GetSize(Guid id);
}