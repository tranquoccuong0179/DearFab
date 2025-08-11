namespace DearFab_Model.Payload.Response.Auth;

public class AuthenticateResponse
{
    public string? AccessToken { get; set; }
    
    public Guid AccountId { get; set; }
    
    public string? Phone { get; set; }
    
    public string? Email { get; set; }
    
    public string? FullName { get; set; }
    
    public string? Role { get; set; }
}