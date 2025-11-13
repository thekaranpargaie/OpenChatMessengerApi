using Chat.Domain;
using Chat.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chat.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChannelsController : ControllerBase
{
    private readonly ChatDbContext _context;
    private readonly ILogger<ChannelsController> _logger;

    public ChannelsController(ChatDbContext context, ILogger<ChannelsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Channel>>> GetChannels()
    {
        return await _context.Channels
            .Where(c => c.IsActive)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Channel>> GetChannel(Guid id)
    {
        var channel = await _context.Channels
            .Include(c => c.Members)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (channel == null)
        {
            return NotFound();
        }

        return channel;
    }

    [HttpPost]
    public async Task<ActionResult<Channel>> CreateChannel(Channel channel)
    {
        channel.Id = Guid.NewGuid();
        channel.CreatedAt = DateTime.UtcNow;
        channel.IsActive = true;

        _context.Channels.Add(channel);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetChannel), new { id = channel.Id }, channel);
    }

    [HttpPost("{channelId}/join")]
    public async Task<ActionResult> JoinChannel(Guid channelId, [FromBody] Guid userId)
    {
        var channel = await _context.Channels.FindAsync(channelId);
        if (channel == null)
        {
            return NotFound();
        }

        var existingMember = await _context.ChannelMembers
            .FirstOrDefaultAsync(m => m.ChannelId == channelId && m.UserId == userId);

        if (existingMember != null)
        {
            return BadRequest("User already a member");
        }

        var member = new ChannelMember
        {
            Id = Guid.NewGuid(),
            ChannelId = channelId,
            UserId = userId,
            JoinedAt = DateTime.UtcNow
        };

        _context.ChannelMembers.Add(member);
        await _context.SaveChangesAsync();

        return Ok();
    }
}
