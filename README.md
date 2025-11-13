# OpenChatMessengerApi

A modern, scalable, real-time chat platform built with .NET 8 and Docker Compose, featuring a beautiful Messenger-style UI.

## ✨ Features

### 🎨 Modern Chat UI *(NEW!)*
- **Facebook Messenger-Style Interface**: Clean, modern design with smooth animations
- **Dark/Light Themes**: Toggle between themes with persistent preference
- **Responsive Design**: Seamless experience on desktop, tablet, and mobile
- **Real-Time Updates**: Instant message delivery with typing indicators
- **Audio Notifications**: Pleasant sound effects for send/receive events
- **Browser Notifications**: Get notified even when tab is inactive

### 🔐 Authentication & Security *(NEW!)*
- **JWT-Based Authentication**: Secure token-based auth system
- **Session Persistence**: Stay logged in across browser sessions
- **Secure Storage**: Tokens stored safely in localStorage
- **User Profiles**: Avatar, status, and profile information

### 💬 Core Functionality
- **Real-Time Messaging**: Instant message delivery using SignalR
- **World Chat**: Global public channel for all users
- **Nearby Users**: Discover and chat with users in your geographic area (IP-based geolocation)
- **Presence Tracking**: Online/offline/typing status indicators
- **Message Archiving**: Automated archiving of old messages to filesystem
- **Webhook Support**: External integrations with HMAC-signed webhooks

### Technical Architecture
- **Microservices Architecture**: Modular services orchestrated with Docker Compose
  - User Service: Authentication, user profiles, and geolocation
  - Chat Service: Messages, channels, and SignalR hubs
  - Presence Service: Online status and typing indicators
  - Webhook Service: External notification system
  - Archive Service: Message archiving to compressed files
- **Database**: PostgreSQL for active data
- **File Storage**: Compressed NDJSON (.ndjson.gz) for archived messages
- **UI**: Blazor Server for interactive web interface

## 🛠️ Technologies Used

- **.NET 8**: Latest .NET framework
- **Blazor Server**: Interactive server-side UI framework
- **SignalR**: Real-time bidirectional communication
- **Docker Compose**: Container orchestration
- **PostgreSQL**: Relational database
- **Entity Framework Core**: ORM
- **JWT**: Secure authentication tokens
- **CSS Custom Properties**: Dynamic theming system
- **Web Audio API**: Programmatic sound generation
- **Notifications API**: Browser push notifications
- **IP Geolocation**: Free IP-API service

## 🚀 Getting Started

### Prerequisites
- Docker Desktop or Docker Engine
- Docker Compose V2

### Quick Start

1. Clone the repository:
```bash
git clone https://github.com/thekaranpargaie/OpenChatMessengerApi.git
cd OpenChatMessengerApi
```

2. Build and run all services:
```bash
docker-compose up --build
```

3. Access the application:
   - Chat UI: http://localhost:5000
   - User API Swagger: http://localhost:5001/swagger
   - Chat API Swagger: http://localhost:5002/swagger

For more detailed Docker Compose instructions, see [DOCKER.md](DOCKER.md).

## Project Structure

```
OpenChatMessengerApi/
├── Services/
│   ├── User/                     # User management service
│   ├── Chat/                     # Chat and messaging service
│   ├── Presence/                 # Presence tracking service
│   ├── Webhook/                  # Webhook notification service
│   └── Archive/                  # Message archiving service
├── UI/
│   └── ChatUI/                   # Blazor web interface
├── Base/                         # Base infrastructure
├── Shared/                       # Shared utilities
├── docker-compose.yml            # Docker Compose configuration
└── docker-compose.dcproj         # Visual Studio Docker integration
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
- [x] Modern Messenger-style UI *(NEW!)*
- [x] Dark/Light theme toggle *(NEW!)*
- [x] Audio & Browser notifications *(NEW!)*
- [x] Responsive mobile design *(NEW!)*
- [x] Typing indicators *(NEW!)*
- [x] IP-based geolocation
- [x] Docker Compose orchestration
- [x] Database migrations (User and Chat services)
- [ ] Nearby users filtering (geolocation-based)
- [ ] Direct messaging (1:1 chats)
- [ ] Private channels with invites
- [ ] Read receipts
- [ ] JWT token issuance (in progress)
- [ ] Comprehensive tests
- [ ] Production deployment configuration

## 📖 Documentation

For detailed information about the new Chat UI features, customization options, and troubleshooting, see:

**[UI_GUIDE.md](UI_GUIDE.md)** - Complete guide to the modern chat interface

Topics covered:
- Feature overview
- Getting started guide
- UI component details
- Keyboard shortcuts
- Theme customization
- Audio notification configuration
- Mobile experience
- Troubleshooting
- Future enhancements

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

See LICENSE.txt for details.

## Architecture Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                     Docker Compose Network                   │
└─────────────────────────────────────────────────────────────┘
                              │
        ┌─────────────────────┼─────────────────────┐
        │                     │                     │
   ┌────▼────┐         ┌──────▼──────┐      ┌──────▼──────┐
   │ ChatUI  │         │  User API   │      │  Chat API   │
   │ (Blazor)│────────▶│  (Auth +    │      │ (Messages + │
   │  :5000  │         │  Profiles)  │      │  SignalR)   │
   └─────────┘         │   :5001     │      │   :5002     │
                       └──────┬──────┘      └──────┬──────┘
                              │                    │
                        ┌─────▼────────────────────▼─────┐
                        │    PostgreSQL :5432            │
                        │  (userdb + chatdb)             │
                        └────────────────────────────────┘
                              
   ┌─────────────┐      ┌─────────────┐    ┌─────────────┐
   │ Presence    │      │  Webhook    │    │  Archive    │
   │  Service    │◄────▶│   Service   │    │  Service    │
   │   :5003     │      │    :5004    │    │   :5005     │
   └──────┬──────┘      └─────────────┘    └──────┬──────┘
          │                                        │
   ┌──────▼──────┐                          ┌─────▼───────┐
   │ Redis :6379 │                          │ Filesystem  │
   └─────────────┘                          │  (NDJSON)   │
                                            └─────────────┘
```

## Development

### Building the Solution
```bash
dotnet build
```

### Running with Docker Compose
```bash
docker-compose up --build
```

### Visual Studio
Open the solution in Visual Studio and set `docker-compose` as the startup project.

### Database Migrations
Migrations are automatically applied when services start. To create new migrations:

```bash
# User service
cd Services/User/User.Infrastructure
dotnet ef migrations add <MigrationName> --context UserDb

# Chat service
cd Services/Chat/Chat.Infrastructure
dotnet ef migrations add <MigrationName> --context ChatDbContext
```

## Support

For issues and questions, please open an issue on GitHub. 
