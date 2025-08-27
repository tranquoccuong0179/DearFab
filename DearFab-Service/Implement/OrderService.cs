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
            Phone = account.Phone,
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
                Id = order.Id,
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

        IPaginate<GetOrderResponse> orders = new Paginate<GetOrderResponse>();

        if (account.Role.Equals(RoleEnum.Admin.GetDescriptionFromEnum()))
        {
            orders = await _unitOfWork.GetRepository<Order>().GetPagingListAsync(
                selector: o => new GetOrderResponse()
                {
                    Id = o.Id,
                    FullName = o.Account.FullName,
                    Address = o.Address,
                    Phone = o.Phone,
                    TotalPrice = o.TotalPrice,
                    Status = o.Status,
                },
                predicate: o => o.IsActive == true,
                include: o => o.Include(o => o.Account),
                orderBy: o => o.OrderByDescending(o => o.CreateAt),
                page: page,
                size: size);
        }

        else
        {
            orders = await _unitOfWork.GetRepository<Order>().GetPagingListAsync(
                selector: o => new GetOrderResponse()
                {
                    Id = o.Id,
                    FullName = o.Account.FullName,
                    Address = o.Address,
                    Phone = o.Phone,
                    TotalPrice = o.TotalPrice,
                    Status = o.Status,
                },
                predicate: o => o.AccountId.Equals(accountId) && o.IsActive == true,
                include: o => o.Include(o => o.Account),
                orderBy: o => o.OrderByDescending(o => o.CreateAt),
                page: page,
                size: size);
        }

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

        var order = new GetOrderDetailResponse();
        if (account.Role.Equals(RoleEnum.Admin.GetDescriptionFromEnum()))
        {
            order = await _unitOfWork.GetRepository<Order>().SingleOrDefaultAsync(
                selector: o => new GetOrderDetailResponse()
                {
                    Id = o.Id,
                    FullName = o.Account.FullName,
                    Address = o.Address,
                    TotalPrice = o.TotalPrice,
                    Phone = o.Phone,
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
                predicate: o => o.Id.Equals(id) && o.IsActive == true,
                include: o => o.Include(o => o.OrderItems).ThenInclude(oi => oi.ProductSize).ThenInclude(ps => ps.Product).Include(o => o.Account));
        }

        else
        {
            order = await _unitOfWork.GetRepository<Order>().SingleOrDefaultAsync(
                selector: o => new GetOrderDetailResponse()
                {
                    Id = o.Id,
                    FullName = o.Account.FullName,
                    Address = o.Address,
                    Phone = o.Phone,
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
        }

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

    public async Task<BaseResponse<GetOrderResponse>> ChangeStatus(Guid id)
    {
        var order = await _unitOfWork.GetRepository<Order>().SingleOrDefaultAsync(
            predicate: o => o.Id.Equals(id) && o.IsActive == true);

        if (order == null)
        {
            return new BaseResponse<GetOrderResponse>()
            {
                Status = StatusCodes.Status404NotFound,
                Message = "Không tìm thấy đơn hàng",
                Data = null
            };
        }

        if (order.Status.Equals(StatusEnum.Pending.GetDescriptionFromEnum()))
        {
            return new BaseResponse<GetOrderResponse>()
            {
                Status = StatusCodes.Status400BadRequest,
                Message = "Đơn hàng này chưa được thanh toán",
                Data = null
            };
        }
        if (order.Status.Equals(StatusEnum.Completed.GetDescriptionFromEnum()))
        {
            return new BaseResponse<GetOrderResponse>()
            {
                Status = StatusCodes.Status400BadRequest,
                Message = "Đơn hàng này đã hoàn thành",
                Data = null
            };
        }
        
        order.Status = StatusEnum.Completed.GetDescriptionFromEnum();
        
        _unitOfWork.GetRepository<Order>().UpdateAsync(order);
        await _unitOfWork.CommitAsync();

        return new BaseResponse<GetOrderResponse>()
        {
            Status = StatusCodes.Status200OK,
            Message = "Cập nhật trạng thái đơn hàng thành công",
            Data = new GetOrderResponse()
            {
                Id = order.Id,
                Address = order.Address,
                TotalPrice = order.TotalPrice,
                Status = order.Status
            }
        };
    }
}