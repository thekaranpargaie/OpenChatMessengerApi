using Microsoft.AspNetCore.Mvc;
using Webhook.Api.Models;
using Webhook.Api.Services;

namespace Webhook.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebhooksController : ControllerBase
{
    private readonly WebhookService _webhookService;
    private readonly ILogger<WebhooksController> _logger;

    public WebhooksController(WebhookService webhookService, ILogger<WebhooksController> logger)
    {
        _webhookService = webhookService;
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<IEnumerable<WebhookRegistration>> GetWebhooks()
    {
        return Ok(_webhookService.GetWebhooks());
    }

    [HttpPost]
    public ActionResult<WebhookRegistration> RegisterWebhook([FromBody] RegisterWebhookRequest request)
    {
        var webhook = _webhookService.RegisterWebhook(request.Url, request.Event, request.Secret);
        return CreatedAtAction(nameof(GetWebhooks), new { id = webhook.Id }, webhook);
    }

    [HttpDelete("{id}")]
    public ActionResult RemoveWebhook(Guid id)
    {
        var removed = _webhookService.RemoveWebhook(id);
        if (!removed)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpPost("trigger")]
    public async Task<ActionResult> TriggerWebhook([FromBody] TriggerWebhookRequest request)
    {
        await _webhookService.TriggerWebhooks(request.Event, request.Data);
        return Ok();
    }
}

public record RegisterWebhookRequest(string Url, string Event, string? Secret);
public record TriggerWebhookRequest(string Event, object Data);
