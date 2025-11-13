using Presence.Api.Models;
using System.Collections.Concurrent;

namespace Presence.Api.Services;

public class PresenceService
{
    private readonly ConcurrentDictionary<string, UserPresence> _userPresences = new();
    private readonly ILogger<PresenceService> _logger;

    public PresenceService(ILogger<PresenceService> logger)
    {
        _logger = logger;
    }

    public void SetUserOnline(string userId, string? channelId = null)
    {
        var presence = new UserPresence
        {
            UserId = userId,
            Status = "online",
            LastSeen = DateTime.UtcNow,
            CurrentChannel = channelId
        };

        _userPresences.AddOrUpdate(userId, presence, (key, old) => presence);
        _logger.LogInformation("User {UserId} is now online", userId);
    }

    public void SetUserOffline(string userId)
    {
        if (_userPresences.TryGetValue(userId, out var presence))
        {
            presence.Status = "offline";
            presence.LastSeen = DateTime.UtcNow;
            _logger.LogInformation("User {UserId} is now offline", userId);
        }
    }

    public void SetUserTyping(string userId, string channelId)
    {
        if (_userPresences.TryGetValue(userId, out var presence))
        {
            presence.LastSeen = DateTime.UtcNow;
        }
    }

    public UserPresence? GetUserPresence(string userId)
    {
        _userPresences.TryGetValue(userId, out var presence);
        return presence;
    }

    public IEnumerable<UserPresence> GetOnlineUsers()
    {
        return _userPresences.Values.Where(p => p.Status == "online");
    }

    public IEnumerable<UserPresence> GetOnlineUsersInChannel(string channelId)
    {
        return _userPresences.Values
            .Where(p => p.Status == "online" && p.CurrentChannel == channelId);
    }
}
