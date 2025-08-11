using DearFab_Model.Entity;
using DearFab_Model.Paginate;
using DearFab_Model.Payload.Request.Size;
using DearFab_Model.Payload.Response;
using DearFab_Model.Payload.Response.Size;
using DearFab_Model.Utils;
using DearFab_Repository.Interface;
using DearFab_Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DearFab_Service.Implement;

public class SizeService : BaseService<SizeService>, ISizeService
{
    public SizeService(IUnitOfWork<DearFabContext> unitOfWork, ILogger<SizeService> logger, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, httpContextAccessor)
    {
    }

    public async Task<BaseResponse<CreateSizeResponse>> CreateSize(CreateSizeRequest request)
    {
        var sizeExist = await _unitOfWork.GetRepository<Size>().SingleOrDefaultAsync(
            predicate: s => s.Label.Equals(request.Label));

        if (sizeExist != null)
        {
            return new BaseResponse<CreateSizeResponse>()
            {
                Status = StatusCodes.Status400BadRequest,
                Message = "Kích thước đã tồn tại",
                Data = null,
            };
        }
        
        Size size = new Size()
        {
            Id = Guid.NewGuid(),
            Label = request.Label,
            IsActive = true,
            CreateAt = TimeUtil.GetCurrentSEATime()
        };
        
        await _unitOfWork.GetRepository<Size>().InsertAsync(size);
        
        var isSuccess = await _unitOfWork.CommitAsync() > 0;
        
        if (!isSuccess)
        {
            throw new Exception("Một lỗi đã xảy ra trong quá trình tạo kích thước");
        }

        return new BaseResponse<CreateSizeResponse>()
        {
            Status = StatusCodes.Status200OK,
            Message = "Tạo kích thước thành công",
            Data = new CreateSizeResponse()
            {
                Label = size.Label,
            }
        };
    }

    public async Task<BaseResponse<IPaginate<GetSizeResponse>>> GetSizes(int page, int size)
    {
        var sizes = await _unitOfWork.GetRepository<Size>().GetPagingListAsync(
            selector: s =>  new GetSizeResponse()
            {
              Id  = s.Id,
              Label = s.Label,
            },
            predicate: s => s.IsActive == true,
            orderBy: s => s.OrderByDescending(s => s.Label),
            page: page,
            size: size);

        return new BaseResponse<IPaginate<GetSizeResponse>>()
        {
            Status = StatusCodes.Status200OK,
            Message = "Lấy danh sách thông tin kích thước thành công",
            Data = sizes,
        };
    }

    public async Task<BaseResponse<GetSizeResponse>> GetSize(Guid id)
    {
        var size = await _unitOfWork.GetRepository<Size>().SingleOrDefaultAsync(
            selector: s =>  new GetSizeResponse()
            {
                Id  = s.Id,
                Label = s.Label,
            },
            predicate: s => s.IsActive == true && s.Id.Equals(id));

        if (size == null)
        {
            return new BaseResponse<GetSizeResponse>()
            {
                Status = StatusCodes.Status404NotFound,
                Message = "Kích thước không tồn tại",
                Data = null,
            };
        }
        
        return new BaseResponse<GetSizeResponse>()
        {
            Status = StatusCodes.Status200OK,
            Message = "Lấy thông tin kích thước thành công",
            Data = size
        };
    }
}