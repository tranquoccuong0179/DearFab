using DearFab_Model.Entity;
using DearFab_Model.Enum;
using DearFab_Model.Payload.Request.Auth;
using DearFab_Model.Payload.Response;
using DearFab_Model.Payload.Response.Auth;
using DearFab_Model.Utils;
using DearFab_Repository.Interface;
using DearFab_Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace DearFab_Service.Implement;

public class AuthService : BaseService<AuthService>, IAuthService
{
    public AuthService(IUnitOfWork<DearFabContext> unitOfWork, ILogger<AuthService> logger, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, httpContextAccessor)
    {
    }

    public async Task<BaseResponse<AuthenticateResponse>> Authenticate(AuthenticateRequest request)
    {
            Expression<Func<Account, bool>> searchFilter = p =>
                (p.Email.Equals(request.Email)) &&
                p.Password.Equals(PasswordUtil.HashPassword(request.Password)) &&
                (p.Role == RoleEnum.Admin.GetDescriptionFromEnum() ||
                 p.Role == RoleEnum.User.GetDescriptionFromEnum()) &&
                p.IsActive == true &&
                p.DeleteAt == null;
                Account account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: searchFilter);
                if (account == null)
                {
                    return new BaseResponse<AuthenticateResponse>()
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Tài khoản hoặc mật khẩu không đúng",
                        Data = null
                    };
                }

            RoleEnum role = EnumUtil.ParseEnum<RoleEnum>(account.Role);
            Tuple<string, Guid> guildClaim = new Tuple<string, Guid>("accountId", account.Id);
            var token = JwtUtil.GenerateJwtToken(account, guildClaim);
            
            var authenticateResponse = new AuthenticateResponse()
            {
                AccessToken = token,
                Email = account.Email,
                Role = role.GetDescriptionFromEnum(),
                FullName = account.FullName,
                Phone = account.Phone,
                AccountId = account.Id
            };

            return new BaseResponse<AuthenticateResponse>()
            {
                Status = StatusCodes.Status200OK,
                Message = "Đăng nhập thành công",
                Data = authenticateResponse
            };
    }
}