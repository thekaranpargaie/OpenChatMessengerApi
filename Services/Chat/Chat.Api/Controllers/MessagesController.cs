using Chat.Domain;
using Chat.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chat.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly ChatDbContext _context;
    private readonly ILogger<MessagesController> _logger;

    public MessagesController(ChatDbContext context, ILogger<MessagesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("channel/{channelId}")]
    public async Task<ActionResult<IEnumerable<Message>>> GetMessages(
        Guid channelId,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50)
    {
        var messages = await _context.Messages
            .Where(m => m.ChannelId == channelId && !m.IsArchived)
            .OrderByDescending(m => m.SentAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return Ok(messages.OrderBy(m => m.SentAt));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Message>> GetMessage(Guid id)
    {
        var message = await _context.Messages.FindAsync(id);

        if (message == null)
        {
            return NotFound();
        }

        return message;
    }
}
