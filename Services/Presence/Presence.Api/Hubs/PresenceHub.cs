using Microsoft.AspNetCore.SignalR;
using Presence.Api.Services;

namespace Presence.Api.Hubs;

public class PresenceHub : Hub
{
    private readonly PresenceService _presenceService;
    private readonly ILogger<PresenceHub> _logger;

    public PresenceHub(PresenceService presenceService, ILogger<PresenceHub> logger)
    {
        _presenceService = presenceService;
        _logger = logger;
    }

    public async Task SetOnline(string userId, string? channelId = null)
    {
        _presenceService.SetUserOnline(userId, channelId);
        await Clients.All.SendAsync("UserOnline", userId);
    }

    public async Task SetOffline(string userId)
    {
        _presenceService.SetUserOffline(userId);
        await Clients.All.SendAsync("UserOffline", userId);
    }

    public async Task NotifyTyping(string userId, string channelId)
    {
        _presenceService.SetUserTyping(userId, channelId);
        await Clients.Others.SendAsync("UserTyping", userId, channelId);
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected to Presence: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client disconnected from Presence: {ConnectionId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}
