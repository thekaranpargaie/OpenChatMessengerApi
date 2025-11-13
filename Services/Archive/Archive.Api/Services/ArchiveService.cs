using System.IO.Compression;
using System.Text.Json;
using Archive.Api.Models;

namespace Archive.Api.Services;

public class ArchiveService
{
    private readonly string _archivePath;
    private readonly ILogger<ArchiveService> _logger;
    private readonly List<ArchiveMetadata> _metadata = new();

    public ArchiveService(IConfiguration configuration, ILogger<ArchiveService> logger)
    {
        _archivePath = configuration.GetValue<string>("ArchivePath") ?? "/archives";
        _logger = logger;
        
        // Ensure archive directory exists
        Directory.CreateDirectory(_archivePath);
    }

    public async Task<ArchiveMetadata> ArchiveMessages(Guid channelId, IEnumerable<ArchivedMessage> messages)
    {
        var messageList = messages.ToList();
        if (!messageList.Any())
        {
            throw new InvalidOperationException("No messages to archive");
        }

        var startDate = messageList.Min(m => m.SentAt);
        var endDate = messageList.Max(m => m.SentAt);
        
        var channelDir = Path.Combine(_archivePath, $"channel_{channelId}");
        Directory.CreateDirectory(channelDir);

        var fileName = $"{startDate:yyyy-MM-dd}_to_{endDate:yyyy-MM-dd}.ndjson.gz";
        var filePath = Path.Combine(channelDir, fileName);

        // Write messages as newline-delimited JSON and compress
        await using (var fileStream = File.Create(filePath))
        await using (var gzipStream = new GZipStream(fileStream, CompressionMode.Compress))
        await using (var writer = new StreamWriter(gzipStream))
        {
            foreach (var message in messageList)
            {
                var json = JsonSerializer.Serialize(message);
                await writer.WriteLineAsync(json);
            }
        }

        var metadata = new ArchiveMetadata
        {
            Id = Guid.NewGuid(),
            ChannelId = channelId,
            StartDate = startDate,
            EndDate = endDate,
            FilePath = filePath,
            MessageCount = messageList.Count,
            ArchivedAt = DateTime.UtcNow
        };

        _metadata.Add(metadata);
        _logger.LogInformation("Archived {Count} messages from {Start} to {End} for channel {ChannelId}",
            messageList.Count, startDate, endDate, channelId);

        return metadata;
    }

    public async Task<IEnumerable<ArchivedMessage>> RetrieveArchivedMessages(Guid channelId, DateTime startDate, DateTime endDate)
    {
        var relevantArchives = _metadata
            .Where(m => m.ChannelId == channelId &&
                       m.StartDate <= endDate &&
                       m.EndDate >= startDate)
            .ToList();

        var messages = new List<ArchivedMessage>();

        foreach (var archive in relevantArchives)
        {
            if (!File.Exists(archive.FilePath))
            {
                _logger.LogWarning("Archive file not found: {FilePath}", archive.FilePath);
                continue;
            }

            await using var fileStream = File.OpenRead(archive.FilePath);
            await using var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress);
            using var reader = new StreamReader(gzipStream);

            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                var message = JsonSerializer.Deserialize<ArchivedMessage>(line);
                if (message != null && message.SentAt >= startDate && message.SentAt <= endDate)
                {
                    messages.Add(message);
                }
            }
        }

        return messages.OrderBy(m => m.SentAt);
    }

    public IEnumerable<ArchiveMetadata> GetArchiveMetadata(Guid? channelId = null)
    {
        return channelId.HasValue
            ? _metadata.Where(m => m.ChannelId == channelId.Value)
            : _metadata;
    }
}
