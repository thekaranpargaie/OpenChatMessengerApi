namespace Presence.Api.Models;

public class UserPresence
{
    public required string UserId { get; set; }
    public required string Status { get; set; } // "online", "offline", "away"
    public DateTime LastSeen { get; set; }
    public string? CurrentChannel { get; set; }
}
