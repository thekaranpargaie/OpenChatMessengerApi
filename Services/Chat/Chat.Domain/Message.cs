namespace Chat.Domain;

public class Message
{
    public Guid Id { get; set; }
    public Guid ChannelId { get; set; }
    public Guid UserId { get; set; }
    public required string Content { get; set; }
    public DateTime SentAt { get; set; }
    public bool IsArchived { get; set; }
    
    public Channel? Channel { get; set; }
}
