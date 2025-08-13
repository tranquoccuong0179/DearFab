using DearFab_Model.Entity;
using DearFab_Model.Paginate;
using DearFab_Model.Payload.Request.Review;
using DearFab_Model.Payload.Response;
using DearFab_Model.Payload.Response.Review;
using DearFab_Model.Utils;
using DearFab_Repository.Interface;
using DearFab_Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DearFab_Service.Implement;

public class ReviewService : BaseService<ReviewService>, IReviewService
{
    public ReviewService(IUnitOfWork<DearFabContext> unitOfWork, ILogger<ReviewService> logger, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, httpContextAccessor)
    {
    }


    public async Task<BaseResponse<CreateReviewResponse>> CreateReview(Guid id, CreateReviewRequest createReviewRequest)
    {
        Guid? accountId = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);

        var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
            predicate: a => a.Id.Equals(accountId) && a.IsActive == true);

        if (account == null)
        {
            return new BaseResponse<CreateReviewResponse>()
            {
                Status = StatusCodes.Status404NotFound,
                Message = "Không tìm thấy thông tin người dùng",
                Data = null
            };
        }

        var product = await _unitOfWork.GetRepository<Product>().SingleOrDefaultAsync(
            predicate: a => a.Id.Equals(id) && a.IsActive == true);

        if (product == null)
        {
            return new BaseResponse<CreateReviewResponse>()
            {
                Status = StatusCodes.Status404NotFound,
                Message = "Không tìm thấy sản phẩm",
                Data = null
            };
        }

        var review = new Review
        {
            Id = Guid.NewGuid(),
            AccountId = account.Id,
            ProductId = product.Id,
            Rating = createReviewRequest.Rating,
            Content = createReviewRequest.Content,
            IsActive = true,
            CreateAt = TimeUtil.GetCurrentSEATime()
        };
        
        await _unitOfWork.GetRepository<Review>().InsertAsync(review);
        var isSuccess = await _unitOfWork.CommitAsync() > 0;
        
        if (!isSuccess)
        {
            throw new Exception("Một lỗi đã xảy ra trong quá trình tạo đánh giá");
        }

        return new BaseResponse<CreateReviewResponse>()
        {
            Status = StatusCodes.Status200OK,
            Message = "Tạo đánh giá thành công",
            Data = new CreateReviewResponse()
            {
                AccountId = account.Id,
                ProductId = product.Id,
                Rating = review.Rating,
                Content = review.Content
            }
        };
    }

    public async Task<BaseResponse<IPaginate<GetReviewResponse>>> GetAllReviews(Guid id, int page, int size)
    {
        var product = await _unitOfWork.GetRepository<Product>().SingleOrDefaultAsync(
            predicate: a => a.Id.Equals(id) && a.IsActive == true);
        if (product == null)
        {
            return new BaseResponse<IPaginate<GetReviewResponse>>()
            {
                Status = StatusCodes.Status404NotFound,
                Message = "Không tìm thấy sản phẩm",
                Data = null
            };
        }

        var reviews = await _unitOfWork.GetRepository<Review>().GetPagingListAsync(
            selector: r => new GetReviewResponse()
            {
                Id = r.Id,
                Content = r.Content,
                Rating = r.Rating,
                FullName = r.Account.FullName,
                CreateAt = r.CreateAt
            },
            predicate: r => r.ProductId.Equals(product.Id) && r.IsActive == true,
            include: r => r.Include(r => r.Account),
            page: page,
            size: size);

        return new BaseResponse<IPaginate<GetReviewResponse>>()
        {
            Status = StatusCodes.Status200OK,
            Message = "Lấy danh sách đánh giá sản phẩm thành công",
            Data = reviews
        };
    }
}