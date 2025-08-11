namespace DearFab_Model.Payload.Request.Auth;

public class AuthenticateRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}