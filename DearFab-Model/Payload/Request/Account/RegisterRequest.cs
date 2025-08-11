namespace DearFab_Model.Payload.Request.Account;

public class RegisterRequest
{
    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;
    
    public string FullName { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Phone { get; set; } = null!;
}