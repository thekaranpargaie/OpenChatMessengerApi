namespace Chat.Domain;

public class Channel
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Type { get; set; } // "world", "private", "nearby"
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    
    public ICollection<Message> Messages { get; set; } = new List<Message>();
    public ICollection<ChannelMember> Members { get; set; } = new List<ChannelMember>();
}
