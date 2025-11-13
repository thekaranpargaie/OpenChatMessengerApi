using Microsoft.AspNetCore.Mvc;
using Archive.Api.Models;
using Archive.Api.Services;

namespace Archive.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArchiveController : ControllerBase
{
    private readonly ArchiveService _archiveService;
    private readonly ILogger<ArchiveController> _logger;

    public ArchiveController(ArchiveService archiveService, ILogger<ArchiveController> logger)
    {
        _archiveService = archiveService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<ArchiveMetadata>> ArchiveMessages([FromBody] ArchiveRequest request)
    {
        try
        {
            var metadata = await _archiveService.ArchiveMessages(request.ChannelId, request.Messages);
            return Ok(metadata);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error archiving messages");
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("retrieve")]
    public async Task<ActionResult<IEnumerable<ArchivedMessage>>> RetrieveMessages(
        [FromQuery] Guid channelId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var messages = await _archiveService.RetrieveArchivedMessages(channelId, startDate, endDate);
        return Ok(messages);
    }

    [HttpGet("metadata")]
    public ActionResult<IEnumerable<ArchiveMetadata>> GetMetadata([FromQuery] Guid? channelId = null)
    {
        var metadata = _archiveService.GetArchiveMetadata(channelId);
        return Ok(metadata);
    }
}

public record ArchiveRequest(Guid ChannelId, List<ArchivedMessage> Messages);
