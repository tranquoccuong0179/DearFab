using DearFab_Model.Entity;
using DearFab_Repository.Interface;
using DearFab_Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DearFab_Service.Implement;

public class ReviewService : BaseService<ReviewService>, IReviewService
{
    public ReviewService(IUnitOfWork<DearFabContext> unitOfWork, ILogger<ReviewService> logger, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, httpContextAccessor)
    {
    }
    
    
}