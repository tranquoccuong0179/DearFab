using DearFab_Model.Paginate;
using DearFab_Model.Payload.Request.Account;
using DearFab_Model.Payload.Response;
using DearFab_Model.Payload.Response.Account;

namespace DearFab_Service.Interface;

public interface IAccountService
{
    Task<BaseResponse<RegisterResponse>> Register(RegisterRequest request);
    
    Task<BaseResponse<IPaginate<GetAccountResponse>>> GetAllAccounts(int page, int size);
    
    Task<BaseResponse<GetAccountResponse>> GetAccountById(Guid id);
    
    Task<BaseResponse<bool>> DeleteAccountById(Guid id);

    Task<BaseResponse<GetAccountResponse>> UpdateAccount(UpdateAccountRequest request);
}