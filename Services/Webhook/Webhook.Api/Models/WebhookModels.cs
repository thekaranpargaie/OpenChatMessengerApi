namespace Webhook.Api.Models;

public class WebhookRegistration
{
    public Guid Id { get; set; }
    public required string Url { get; set; }
    public required string Event { get; set; } // "message.created", "user.joined", etc.
    public string? Secret { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class WebhookPayload
{
    public required string Event { get; set; }
    public required object Data { get; set; }
    public DateTime Timestamp { get; set; }
}
