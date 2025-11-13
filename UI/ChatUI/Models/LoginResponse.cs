namespace ChatUI.Models;

public class LoginResponse
{
    public bool Success { get; set; }
    public string Token { get; set; } = "";
    public UserData? UserData { get; set; }
}

public class UserData
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string ContactEmail { get; set; } = "";
    public string? Image { get; set; }
}
