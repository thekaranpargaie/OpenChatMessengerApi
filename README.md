# OpenChatMessengerApi

A modern, scalable, real-time chat platform built with .NET 8 and orchestrated using .NET Aspire.

## Features

### Core Functionality
- **Real-Time Messaging**: Instant message delivery using SignalR
- **World Chat**: Global public channel for all users
- **Nearby Users**: Discover and chat with users in your geographic area (IP-based geolocation)
- **Presence Tracking**: Online/offline/typing status indicators
- **Message Archiving**: Automated archiving of old messages to filesystem
- **Webhook Support**: External integrations with HMAC-signed webhooks

### Technical Architecture
- **Microservices Architecture**: Modular services orchestrated with .NET Aspire
  - User Service: Authentication, user profiles, and geolocation
  - Chat Service: Messages, channels, and SignalR hubs
  - Presence Service: Online status and typing indicators
  - Webhook Service: External notification system
  - Archive Service: Message archiving to compressed files
- **Database**: PostgreSQL for active data
- **File Storage**: Compressed NDJSON (.ndjson.gz) for archived messages
- **UI**: Blazor Server for interactive web interface

## Technologies Used

- **.NET 8**: Latest .NET framework
- **.NET Aspire**: Orchestration and observability
- **SignalR**: Real-time communication
- **PostgreSQL**: Primary database
- **Entity Framework Core**: ORM
- **Blazor Server**: Interactive UI
- **JWT**: Authentication tokens
- **IP Geolocation**: Free IP-API service

## Getting Started

### Prerequisites
- .NET 8 SDK
- Docker (for PostgreSQL via Aspire)
- .NET Aspire workload

### Installation

1. Install .NET Aspire workload:
```bash
dotnet workload install aspire
```

2. Clone the repository:
```bash
git clone https://github.com/thekaranpargaie/OpenChatMessengerApi.git
cd OpenChatMessengerApi
```

3. Run the application:
```bash
cd Aspire/Aspire.AppHost
dotnet run
```

4. Access the Aspire dashboard (usually at http://localhost:15000)

5. Access the Chat UI (check Aspire dashboard for the actual URL)

## Project Structure

```
OpenChatMessengerApi/
├── Aspire/
│   ├── Aspire.AppHost/          # Orchestration host
│   └── Aspire.ServiceDefaults/  # Shared configuration
├── Services/
│   ├── User/                     # User management service
│   ├── Chat/                     # Chat and messaging service
│   ├── Presence/                 # Presence tracking service
│   ├── Webhook/                  # Webhook notification service
│   └── Archive/                  # Message archiving service
├── UI/
│   └── ChatUI/                   # Blazor web interface
├── Base/                         # Base infrastructure
└── Shared/                       # Shared utilities
```

## API Endpoints

### Chat Service
- `GET /api/channels` - List all channels
- `POST /api/channels` - Create a new channel
- `GET /api/messages/channel/{channelId}` - Get messages for a channel
- SignalR Hub: `/chathub`

### Presence Service
- `GET /api/presence/online` - Get online users
- `GET /api/presence/{userId}` - Get user presence
- SignalR Hub: `/presencehub`

### Webhook Service
- `GET /api/webhooks` - List registered webhooks
- `POST /api/webhooks` - Register a webhook
- `DELETE /api/webhooks/{id}` - Remove a webhook

### Archive Service
- `POST /api/archive` - Archive messages
- `GET /api/archive/retrieve` - Retrieve archived messages
- `GET /api/archive/metadata` - Get archive metadata

## Configuration

### Archive Service
Configure the archive path in appsettings.json:
```json
{
  "ArchivePath": "/archives"
}
```

### Message Retention
Messages older than 30 days (configurable) are automatically archived to the filesystem.

## Features Implementation Status

- [x] User authentication and profiles
- [x] Real-time messaging with SignalR
- [x] World Chat channel
- [x] Message persistence
- [x] Presence tracking
- [x] Webhook system with HMAC signing
- [x] Message archiving to filesystem
- [x] Blazor UI
- [x] IP-based geolocation
- [ ] Nearby users filtering (geolocation-based)
- [ ] JWT token issuance
- [ ] Database migrations
- [ ] Comprehensive tests
- [ ] Production deployment configuration

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

See LICENSE.txt for details.

## Architecture Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                      .NET Aspire AppHost                     │
└─────────────────────────────────────────────────────────────┘
                              │
        ┌─────────────────────┼─────────────────────┐
        │                     │                     │
   ┌────▼────┐         ┌──────▼──────┐      ┌──────▼──────┐
   │ ChatUI  │         │  User API   │      │  Chat API   │
   │ (Blazor)│────────▶│  (Auth +    │      │ (Messages + │
   └─────────┘         │  Profiles)  │      │  SignalR)   │
                       └──────┬──────┘      └──────┬──────┘
                              │                    │
                        ┌─────▼────────────────────▼─────┐
                        │       PostgreSQL Database       │
                        └─────────────────────────────────┘
                              
   ┌─────────────┐      ┌─────────────┐    ┌─────────────┐
   │ Presence    │      │  Webhook    │    │  Archive    │
   │  Service    │      │   Service   │    │  Service    │
   └─────────────┘      └─────────────┘    └──────┬──────┘
                                                   │
                                            ┌──────▼──────┐
                                            │ Filesystem  │
                                            │  (NDJSON)   │
                                            └─────────────┘
```

## Development

### Building the Solution
```bash
dotnet build
```

### Running Tests
```bash
dotnet test
```

### Database Migrations
Entity Framework migrations will be added for database schema management.

## Support

For issues and questions, please open an issue on GitHub. 
