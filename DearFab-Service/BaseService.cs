using System.Security.Claims;
using DearFab_Model.Entity;
using DearFab_Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DearFab_Service;

public abstract class BaseService<T> where T : class
{
    protected IUnitOfWork<DearFabContext> _unitOfWork;
    protected ILogger _logger;
    protected IHttpContextAccessor _httpContextAccessor;

    public BaseService(IUnitOfWork<DearFabContext> unitOfWork, ILogger<T> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    protected string GetUsernameFromJwt()
    {
        var claim = _httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
        string username = claim?.Value;
        return username;
    }

    protected string GetRoleFromJwt()
    {
        var claim = _httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.Role);
        string role = claim?.Value;
        return role;
    }

}