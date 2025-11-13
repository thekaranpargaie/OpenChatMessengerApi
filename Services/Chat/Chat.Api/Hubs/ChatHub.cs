using Chat.Domain;
using Chat.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Chat.Api.Hubs;

public class ChatHub : Hub
{
    private readonly ChatDbContext _context;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(ChatDbContext context, ILogger<ChatHub> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task JoinChannel(string channelId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, channelId);
        _logger.LogInformation("User {ConnectionId} joined channel {ChannelId}", Context.ConnectionId, channelId);
    }

    public async Task LeaveChannel(string channelId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, channelId);
        _logger.LogInformation("User {ConnectionId} left channel {ChannelId}", Context.ConnectionId, channelId);
    }

    public async Task SendMessage(string channelId, string userId, string content)
    {
        var message = new Message
        {
            Id = Guid.NewGuid(),
            ChannelId = Guid.Parse(channelId),
            UserId = Guid.Parse(userId),
            Content = content,
            SentAt = DateTime.UtcNow,
            IsArchived = false
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        await Clients.Group(channelId).SendAsync("ReceiveMessage", new
        {
            message.Id,
            message.ChannelId,
            message.UserId,
            message.Content,
            message.SentAt
        });

        _logger.LogInformation("Message sent to channel {ChannelId}", channelId);
    }

    public async Task Typing(string channelId, string userId)
    {
        await Clients.OthersInGroup(channelId).SendAsync("UserTyping", userId);
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}
