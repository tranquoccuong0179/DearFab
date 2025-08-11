using DearFab_Model.Entity;
using DearFab_Model.Enum;
using DearFab_Model.Paginate;
using DearFab_Model.Payload.Request.Account;
using DearFab_Model.Payload.Response;
using DearFab_Model.Payload.Response.Account;
using DearFab_Model.Utils;
using DearFab_Repository.Interface;
using DearFab_Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DearFab_Service.Implement;

public class AccountService : BaseService<AccountService>, IAccountService
{
    public AccountService(IUnitOfWork<DearFabContext> unitOfWork, ILogger<AccountService> logger, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, httpContextAccessor)
    {
    }

    public async Task<BaseResponse<RegisterResponse>> Register(RegisterRequest request)
    {
        var accountList = await _unitOfWork.GetRepository<Account>().GetListAsync();
        
        if (accountList.Any(a => a.Email.Equals(request.Email)))
        {
            throw new BadHttpRequestException("Email đã tồn tại");
        }

        if (accountList.Any(a => a.Phone.Equals(request.Phone)))
        {
            throw new BadHttpRequestException("Số điện thoại đã tồn tại");
        }

        var account = new Account()
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Phone = request.Phone,
            Password = PasswordUtil.HashPassword(request.Password),
            Role = RoleEnum.User.GetDescriptionFromEnum(),
            FullName = request.FullName,
            Address = request.Address,
            IsActive = true,
            CreateAt = TimeUtil.GetCurrentSEATime()
        };
        
        await _unitOfWork.GetRepository<Account>().InsertAsync(account);
        
        var isSuccess = await _unitOfWork.CommitAsync() > 0;
        
        if (!isSuccess)
        {
            throw new Exception("Một lỗi đã xảy ra trong quá trình tạo tài khoản");
        }

        return new BaseResponse<RegisterResponse>()
        {
            Status = StatusCodes.Status200OK,
            Message = "Đăng kí tài khoản thành công",
            Data = new RegisterResponse()
            {
                Address = account.Address,
                Email = account.Email,
                Phone = account.Phone,
                FullName = account.FullName,
                Role = account.Role,
            }
        };
    }

    public async Task<BaseResponse<IPaginate<GetAccountResponse>>> GetAllAccounts(int page, int size)
    {
        var accounts = await _unitOfWork.GetRepository<Account>().GetPagingListAsync(
            selector: a => new GetAccountResponse
            {
                Id = a.Id,
                Email = a.Email,
                Phone = a.Phone,
                FullName = a.FullName,
                Address = a.Address,
            },
            predicate: a => a.Role == RoleEnum.User.GetDescriptionFromEnum() && a.IsActive == true,
            page: page,
            size: size);

        return new BaseResponse<IPaginate<GetAccountResponse>>()
        {
            Status = StatusCodes.Status200OK,
            Message = "Lấy danh sách thông tin người dùng thành công",
            Data = accounts
        };
    }

    public async Task<BaseResponse<GetAccountResponse>> GetAccountById(Guid id)
    {
        var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
            selector: a => new GetAccountResponse()
            {
                Id = a.Id,
                Email = a.Email,
                Phone = a.Phone,
                FullName = a.FullName,
                Address = a.Address,
            },
            predicate: a => a.Role == RoleEnum.User.GetDescriptionFromEnum() && a.IsActive == true && a.Id.Equals(id));

        if (account == null)
        {
            return new BaseResponse<GetAccountResponse>()
            {
                Status = StatusCodes.Status404NotFound,
                Message = "Thông tin người dùng không tồn tại",
                Data = null,
            };
        }

        return new BaseResponse<GetAccountResponse>()
        {
            Status = StatusCodes.Status200OK,
            Message = "Lấy thông tin người dùng thành công",
            Data = account
        };
    }

    public async Task<BaseResponse<bool>> DeleteAccountById(Guid id)
    {
        var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
            predicate: a => a.Id.Equals(id) && a.IsActive == true && a.Role == RoleEnum.User.GetDescriptionFromEnum());

        if (account == null)
        {
            return new BaseResponse<bool>()
            {
                Status = StatusCodes.Status404NotFound,
                Message = "Không tìm thấy thông tin người dùng",
                Data = false
            };
        }
        
        account.IsActive = false;
        account.DeleteAt = TimeUtil.GetCurrentSEATime();
        
        _unitOfWork.GetRepository<Account>().UpdateAsync(account);
        var isSuccess = await _unitOfWork.CommitAsync() > 0;
        if (!isSuccess)
        {
            throw new Exception("Một lỗi đã xảy ra trong quá trình xóa tài khoản");
        }

        return new BaseResponse<bool>()
        {
            Status = StatusCodes.Status200OK,
            Message = "Xóa tài khoản thành công",
            Data = true
        };
    }

    public async Task<BaseResponse<GetAccountResponse>> UpdateAccount(UpdateAccountRequest request)
    {
        Guid? accountId = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);

        var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
            predicate: a => a.Id.Equals(accountId) && a.IsActive == true);

        if (account == null)
        {
            return new BaseResponse<GetAccountResponse>()
            {
                Status = StatusCodes.Status404NotFound,
                Message = "Không tìm thấy thông tin người dùng",
                Data = null
            };
        }
        
        account.FullName = request.FullName ?? account.FullName;
        account.Address = request.Address ?? account.Address;
        
        _unitOfWork.GetRepository<Account>().UpdateAsync(account);
        var isSuccess = await _unitOfWork.CommitAsync() > 0;
        
        if (!isSuccess)
        {
            throw new Exception("Một lỗi đã xảy ra trong quá trình đổi cập nhật thông tin mới");
        }

        return new BaseResponse<GetAccountResponse>()
        {
            Status = StatusCodes.Status200OK,
            Message = "Cập nhật thông tin thành công",
            Data = new GetAccountResponse()
            {
                Id = account.Id,
                FullName = account.FullName,
                Address = account.Address,
                Email = account.Email,
                Phone = account.Phone,
            }
        };
    }
}