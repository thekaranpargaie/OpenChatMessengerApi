# Setup Guide

## Prerequisites

Before you begin, ensure you have the following installed:

1. **.NET 8 SDK** (version 8.0 or later)
   - Download from: https://dotnet.microsoft.com/download/dotnet/8.0
   - Verify installation: `dotnet --version`

2. **Docker Desktop** (for running PostgreSQL)
   - Download from: https://www.docker.com/products/docker-desktop
   - Required for .NET Aspire to manage PostgreSQL containers

3. **.NET Aspire Workload**
   ```bash
   dotnet workload install aspire
   ```

## Installation Steps

### 1. Clone the Repository

```bash
git clone https://github.com/thekaranpargaie/OpenChatMessengerApi.git
cd OpenChatMessengerApi
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Build the Solution

```bash
dotnet build
```

### 4. Run with Aspire

```bash
cd Aspire/Aspire.AppHost
dotnet run
```

This will:
- Start the Aspire dashboard
- Launch PostgreSQL container
- Start all microservices (User, Chat, Presence, Webhook, Archive)
- Start the Blazor UI

### 5. Access the Applications

Once running, you can access:

- **Aspire Dashboard**: Usually at `http://localhost:15000` or `http://localhost:18888`
  - View all running services
  - Monitor health and metrics
  - View logs and traces

- **Chat UI**: Check the Aspire dashboard for the actual URL (typically `http://localhost:5xxx`)

- **Individual Services**: Check Aspire dashboard for each service's endpoint

## Configuration

### Database Connection

The database connection is automatically configured by Aspire. The connection string is injected into services that need it.

### Archive Storage

To customize the archive path, edit `Services/Archive/Archive.Api/appsettings.json`:

```json
{
  "ArchivePath": "/path/to/your/archives",
  "RetentionDays": 30
}
```

### CORS Settings

By default, CORS is configured to allow local development. For production:

1. Update CORS policies in each service's `Program.cs`
2. Restrict allowed origins to your actual domains

## Development Workflow

### Running Individual Services

You can run services individually for development:

```bash
# Run Chat API
cd Services/Chat/Chat.Api
dotnet run

# Run User API
cd Services/User/User.Api
dotnet run
```

### Database Migrations

When you modify domain models, create migrations:

```bash
cd Services/Chat/Chat.Infrastructure
dotnet ef migrations add MigrationName -s ../Chat.Api
dotnet ef database update -s ../Chat.Api
```

### Hot Reload

.NET supports hot reload during development:
- Make code changes
- Changes are automatically applied without restart (in most cases)

## Troubleshooting

### Port Conflicts

If you encounter port conflicts:
1. Check the Aspire dashboard for actual port assignments
2. Modify `launchSettings.json` in individual projects if needed

### Database Issues

If PostgreSQL container fails to start:
1. Ensure Docker Desktop is running
2. Check Docker logs via Aspire dashboard
3. Remove and recreate the container

### SignalR Connection Issues

If SignalR fails to connect:
1. Verify CORS settings allow the UI origin
2. Check that the Chat API is running
3. Verify the hub URL in the UI matches the actual service URL

## Testing the System

### 1. Test Chat Functionality

1. Navigate to the Chat UI
2. Go to the Chat page
3. Select "World Chat" channel
4. Send a message
5. Open another browser window to see real-time updates

### 2. Test Webhooks

```bash
# Register a webhook
curl -X POST http://localhost:<webhook-port>/api/webhooks \
  -H "Content-Type: application/json" \
  -d '{
    "url": "https://webhook.site/unique-id",
    "event": "message.created",
    "secret": "your-secret"
  }'
```

### 3. Test Archive Service

```bash
# Archive messages
curl -X POST http://localhost:<archive-port>/api/archive \
  -H "Content-Type: application/json" \
  -d '{
    "channelId": "channel-guid",
    "messages": [...]
  }'
```

## Next Steps

1. **Implement Authentication**: Add JWT token generation in User API
2. **Add Database Migrations**: Create proper EF migrations for all services
3. **Implement Nearby Users**: Use geolocation data to filter nearby users
4. **Add Tests**: Create unit and integration tests
5. **Configure for Production**: Update settings for production deployment

## Additional Resources

- [.NET Aspire Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/)
- [SignalR Documentation](https://learn.microsoft.com/en-us/aspnet/core/signalr/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [Blazor Documentation](https://learn.microsoft.com/en-us/aspnet/core/blazor/)
