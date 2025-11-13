namespace Archive.Api.Models;

public class ArchivedMessage
{
    public Guid Id { get; set; }
    public Guid ChannelId { get; set; }
    public Guid UserId { get; set; }
    public required string Content { get; set; }
    public DateTime SentAt { get; set; }
}

public class ArchiveMetadata
{
    public Guid Id { get; set; }
    public Guid ChannelId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public required string FilePath { get; set; }
    public long MessageCount { get; set; }
    public DateTime ArchivedAt { get; set; }
}
