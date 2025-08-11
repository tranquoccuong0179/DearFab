using DearFab_Model.Payload.Request.Auth;
using DearFab_Model.Payload.Response;
using DearFab_Model.Payload.Response.Auth;

namespace DearFab_Service.Interface;

public interface IAuthService
{
    Task<BaseResponse<AuthenticateResponse>> Authenticate (AuthenticateRequest request);
}