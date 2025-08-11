using DearFab_Model.Paginate;
using DearFab_Model.Payload.Request.Account;
using DearFab_Model.Payload.Response;
using DearFab_Model.Payload.Response.Account;
using DearFab_Service.Interface;
using DearFab.Constant;
using Microsoft.AspNetCore.Mvc;

namespace DearFab.Controller;

public class AccountController : BaseController<AccountController>
{
    private readonly IAccountService _accountService;
    public AccountController(ILogger<AccountController> logger, IAccountService accountService) : base(logger)
    {
        _accountService = accountService;
    }
    
    [HttpPost(ApiEndPointConstant.Account.RegisterAccount)]
    [ProducesResponseType(typeof(BaseResponse<RegisterResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<RegisterResponse>), StatusCodes.Status500InternalServerError)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> RegisterAccount([FromBody] RegisterRequest request)
    {
        var response = await _accountService.Register(request);
        return StatusCode(response.Status, response);
    }
    
    [HttpGet(ApiEndPointConstant.Account.GetAccounts)]
    [ProducesResponseType(typeof(BaseResponse<IPaginate<GetAccountResponse>>), StatusCodes.Status200OK)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> GetAccounts([FromQuery] int? page, [FromQuery] int? size)
    {
        int pageNumber = page ?? 1;
        int pageSize = size ?? 10;
        var response = await _accountService.GetAllAccounts(pageNumber, pageSize);
        return StatusCode(response.Status, response);
    }
    
    [HttpGet(ApiEndPointConstant.Account.GetAccount)]
    [ProducesResponseType(typeof(BaseResponse<GetAccountResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<GetAccountResponse>), StatusCodes.Status404NotFound)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> GetAccount([FromRoute] Guid id)
    {
        var response = await _accountService.GetAccountById(id);
        return StatusCode(response.Status, response);
    }
    
    [HttpPut(ApiEndPointConstant.Account.UpdateAccount)]
    [ProducesResponseType(typeof(BaseResponse<GetAccountResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<GetAccountResponse>), StatusCodes.Status404NotFound)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> GetAccount([FromBody] UpdateAccountRequest request)
    {
        var response = await _accountService.UpdateAccount(request);
        return StatusCode(response.Status, response);
    }
    
    [HttpDelete(ApiEndPointConstant.Account.DeleteAccount)]
    [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status404NotFound)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> DeleteAccount([FromRoute] Guid id)
    {
        var response = await _accountService.DeleteAccountById(id);
        return StatusCode(response.Status, response);
    }
}