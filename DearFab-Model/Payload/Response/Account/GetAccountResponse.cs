namespace DearFab_Model.Payload.Response.Account;

public class GetAccountResponse
{
    public Guid Id { get; set; }
    
    public string? Email { get; set; }
    
    public string? FullName { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }
}