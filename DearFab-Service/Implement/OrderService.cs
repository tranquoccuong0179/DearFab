using DearFab_Model.Entity;
using DearFab_Model.Enum;
using DearFab_Model.Paginate;
using DearFab_Model.Payload.Request.Order;
using DearFab_Model.Payload.Response;
using DearFab_Model.Payload.Response.Order;
using DearFab_Model.Payload.Response.OrderItem;
using DearFab_Model.Utils;
using DearFab_Repository.Interface;
using DearFab_Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DearFab_Service.Implement;

public class OrderService : BaseService<OrderService>, IOrderService
{
    public OrderService(IUnitOfWork<DearFabContext> unitOfWork, ILogger<OrderService> logger, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, httpContextAccessor)
    {
    }

    public async Task<BaseResponse<CreateOrderResponse>> CreateOrder(CreateOrderRequest request)
    {
        Guid? accountId = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);

        var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
            predicate: a => a.Id.Equals(accountId) && a.IsActive == true);

        if (account == null)
        {
            return new BaseResponse<CreateOrderResponse>()
            {
                Status = StatusCodes.Status404NotFound,
                Message = "Không tìm thấy thông tin người dùng",
                Data = null
            };
        }

        var order = new Order()
        {
            Id = Guid.NewGuid(),
            AccountId = account.Id,
            Status = StatusEnum.Pending.GetDescriptionFromEnum(),
            Address = request.Address,
            TotalPrice = 0,
            IsActive = true,
            CreateAt = TimeUtil.GetCurrentSEATime()
        };
        
        await _unitOfWork.GetRepository<Order>().InsertAsync(order);
        
        var totalPrice = 0;
        var orderItems = new List<OrderItem>();
        foreach (var item in request.OrderItems)
        {
            var productSize = await _unitOfWork.GetRepository<ProductSize>().SingleOrDefaultAsync(
                predicate: p => p.Id.Equals(item.ProductSizeId) && p.IsActive == true);
            if (productSize == null)
            {
                return new BaseResponse<CreateOrderResponse>()
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Không tìm thấy sản phẩm",
                    Data = null
                };
            }

            var orderItem = new OrderItem()
            {
                Id = Guid.NewGuid(),
                ProductSizeId = productSize.Id,
                Quantity = item.Quantity,
                OrderId = order.Id
            };
            orderItems.Add(orderItem);
            totalPrice += (int)productSize.Price * item.Quantity;
        }
        
        order.TotalPrice = totalPrice;
        
        await _unitOfWork.GetRepository<OrderItem>().InsertRangeAsync(orderItems);
        
        var isSuccess = await _unitOfWork.CommitAsync() > 0;
        if (!isSuccess)
        {
            throw new Exception("Một lỗi đã xảy ra trong quá trình tạo đơn hàng");
        }

        return new BaseResponse<CreateOrderResponse>()
        {
            Status = StatusCodes.Status200OK,
            Message = "Tạo đơn hàng thành công",
            Data = new CreateOrderResponse()
            {
                ProductSizeId = orderItems.Select(oi => oi.ProductSizeId).ToList(),
                TotalPrice = totalPrice,
                Address = order.Address,
                Status = order.Status,
            }
        };
    }

    public async Task<BaseResponse<IPaginate<GetOrderResponse>>> GetAllOrder(int page, int size)
    {
        Guid? accountId = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);

        var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
            predicate: a => a.Id.Equals(accountId) && a.IsActive == true);

        if (account == null)
        {
            return new BaseResponse<IPaginate<GetOrderResponse>>()
            {
                Status = StatusCodes.Status404NotFound,
                Message = "Không tìm thấy thông tin người dùng",
                Data = null
            };
        }

        var orders = await _unitOfWork.GetRepository<Order>().GetPagingListAsync(
            selector: o => new GetOrderResponse()
            {
                Id = o.Id,
                Address = o.Address,
                TotalPrice = o.TotalPrice,
                Status = o.Status,
            },
            predicate: o => o.AccountId.Equals(accountId) && o.IsActive == true,
            orderBy: o => o.OrderByDescending(o => o.CreateAt),
            page: page,
            size: size);

        return new BaseResponse<IPaginate<GetOrderResponse>>()
        {
            Status = StatusCodes.Status200OK,
            Message = "Lấy danh sách đơn hàng thành công",
            Data = orders,
        };
    }

    public async Task<BaseResponse<GetOrderDetailResponse>> GetOrderDetail(Guid id)
    {
        Guid? accountId = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);

        var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
            predicate: a => a.Id.Equals(accountId) && a.IsActive == true);

        if (account == null)
        {
            return new BaseResponse<GetOrderDetailResponse>()
            {
                Status = StatusCodes.Status404NotFound,
                Message = "Không tìm thấy thông tin người dùng",
                Data = null
            };
        }

        var order = await _unitOfWork.GetRepository<Order>().SingleOrDefaultAsync(
            selector: o => new GetOrderDetailResponse()
            {
                Id = o.Id,
                Address = o.Address,
                TotalPrice = o.TotalPrice,
                Status = o.Status,
                items = o.OrderItems.Select(oi => new GetOrderItemResponse()
                {
                    Id = oi.Id,
                    Name = oi.ProductSize.Product.Name,
                    Price = oi.ProductSize.Price,
                    Quantity = oi.Quantity,
                    Description = oi.ProductSize.Product.Description,
                    Image = oi.ProductSize.Product.Image,
                }).ToList()
            },
            predicate: o => o.Id.Equals(id) && o.AccountId.Equals(account.Id) && o.IsActive == true,
            include: o => o.Include(o => o.OrderItems).ThenInclude(oi => oi.ProductSize).ThenInclude(ps => ps.Product));

        if (order == null)
        {
            return new BaseResponse<GetOrderDetailResponse>()
            {
                Status = StatusCodes.Status404NotFound,
                Message = "Không tìm thấy đơn hàng",
                Data = null
            };
        }

        return new BaseResponse<GetOrderDetailResponse>()
        {
            Status = StatusCodes.Status200OK,
            Message = "Lấy thông tin đơn hàng thành công",
            Data = order,
        };
    }
}