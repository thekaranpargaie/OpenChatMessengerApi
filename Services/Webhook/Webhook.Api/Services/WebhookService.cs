using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Webhook.Api.Models;

namespace Webhook.Api.Services;

public class WebhookService
{
    private readonly ConcurrentDictionary<Guid, WebhookRegistration> _webhooks = new();
    private readonly HttpClient _httpClient;
    private readonly ILogger<WebhookService> _logger;

    public WebhookService(HttpClient httpClient, ILogger<WebhookService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public WebhookRegistration RegisterWebhook(string url, string eventType, string? secret = null)
    {
        var registration = new WebhookRegistration
        {
            Id = Guid.NewGuid(),
            Url = url,
            Event = eventType,
            Secret = secret,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _webhooks.TryAdd(registration.Id, registration);
        _logger.LogInformation("Webhook registered: {Id} for event {Event}", registration.Id, eventType);
        
        return registration;
    }

    public bool RemoveWebhook(Guid id)
    {
        var removed = _webhooks.TryRemove(id, out _);
        if (removed)
        {
            _logger.LogInformation("Webhook removed: {Id}", id);
        }
        return removed;
    }

    public IEnumerable<WebhookRegistration> GetWebhooks()
    {
        return _webhooks.Values.Where(w => w.IsActive);
    }

    public async Task TriggerWebhooks(string eventType, object data)
    {
        var webhooks = _webhooks.Values.Where(w => w.IsActive && w.Event == eventType);

        var payload = new WebhookPayload
        {
            Event = eventType,
            Data = data,
            Timestamp = DateTime.UtcNow
        };

        var tasks = webhooks.Select(webhook => SendWebhook(webhook, payload));
        await Task.WhenAll(tasks);
    }

    private async Task SendWebhook(WebhookRegistration webhook, WebhookPayload payload)
    {
        try
        {
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            if (!string.IsNullOrEmpty(webhook.Secret))
            {
                var signature = GenerateHmacSignature(json, webhook.Secret);
                content.Headers.Add("X-Webhook-Signature", signature);
            }

            var response = await _httpClient.PostAsync(webhook.Url, content);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Webhook sent successfully to {Url}", webhook.Url);
            }
            else
            {
                _logger.LogWarning("Webhook failed with status {Status} to {Url}", response.StatusCode, webhook.Url);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending webhook to {Url}", webhook.Url);
        }
    }

    private string GenerateHmacSignature(string payload, string secret)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        return Convert.ToHexString(hash).ToLower();
    }
}
