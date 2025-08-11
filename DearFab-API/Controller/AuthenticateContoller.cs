using DearFab_Model.Payload.Request.Auth;
using DearFab_Model.Payload.Response;
using DearFab_Model.Payload.Response.Auth;
using DearFab_Service.Interface;
using DearFab.Constant;
using Microsoft.AspNetCore.Mvc;

namespace DearFab.Controller;

public class AuthenticateContoller : BaseController<AuthenticateContoller>
{
    private readonly IAuthService _authService;

    public AuthenticateContoller(ILogger<AuthenticateContoller> logger, IAuthService authService) : base(logger)
    {
        _authService = authService;
    }

    [HttpPost(ApiEndPointConstant.Authentication.Authenticate)]
    [ProducesResponseType(typeof(BaseResponse<AuthenticateResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<AuthenticateResponse>), StatusCodes.Status400BadRequest)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest request)
    {
        var response = await _authService.Authenticate(request);
        return StatusCode(response.Status, response);
    }
}