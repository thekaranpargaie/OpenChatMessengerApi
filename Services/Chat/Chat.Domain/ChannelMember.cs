namespace Chat.Domain;

public class ChannelMember
{
    public Guid Id { get; set; }
    public Guid ChannelId { get; set; }
    public Guid UserId { get; set; }
    public DateTime JoinedAt { get; set; }
    public DateTime? LastReadAt { get; set; }
    
    public Channel? Channel { get; set; }
}
