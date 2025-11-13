# API Documentation

## Authentication

**Status**: To be implemented

Future authentication will use JWT tokens issued by the User API.

## Services Overview

### User API
Manages user accounts, profiles, and geolocation.

### Chat API
Handles messages, channels, and real-time communication via SignalR.

### Presence API
Tracks online/offline status and typing indicators.

### Webhook API
Manages external webhook integrations.

### Archive API
Handles message archiving to filesystem.

---

## User API

### Endpoints

#### Get User Profile
```http
GET /api/users/{id}
```

#### Update User Profile
```http
PUT /api/users/{id}
Content-Type: application/json

{
  "firstName": "string",
  "lastName": "string",
  "contactEmail": "string",
  "image": "string"
}
```

---

## Chat API

### REST Endpoints

#### List Channels
```http
GET /api/channels
```

**Response:**
```json
[
  {
    "id": "guid",
    "name": "World Chat",
    "type": "world",
    "createdAt": "2025-01-01T00:00:00Z",
    "isActive": true
  }
]
```

#### Get Channel
```http
GET /api/channels/{id}
```

#### Create Channel
```http
POST /api/channels
Content-Type: application/json

{
  "name": "My Channel",
  "type": "private"
}
```

#### Join Channel
```http
POST /api/channels/{channelId}/join
Content-Type: application/json

"user-id-guid"
```

#### Get Messages
```http
GET /api/messages/channel/{channelId}?skip=0&take=50
```

**Response:**
```json
[
  {
    "id": "guid",
    "channelId": "guid",
    "userId": "guid",
    "content": "Hello!",
    "sentAt": "2025-01-01T12:00:00Z",
    "isArchived": false
  }
]
```

### SignalR Hub: `/chathub`

#### Client → Server Methods

**JoinChannel**
```javascript
connection.invoke("JoinChannel", channelId);
```

**LeaveChannel**
```javascript
connection.invoke("LeaveChannel", channelId);
```

**SendMessage**
```javascript
connection.invoke("SendMessage", channelId, userId, content);
```

**Typing**
```javascript
connection.invoke("Typing", channelId, userId);
```

#### Server → Client Events

**ReceiveMessage**
```javascript
connection.on("ReceiveMessage", (message) => {
  // message: { id, channelId, userId, content, sentAt }
});
```

**UserTyping**
```javascript
connection.on("UserTyping", (userId) => {
  // Handle typing indicator
});
```

---

## Presence API

### REST Endpoints

#### Get Online Users
```http
GET /api/presence/online
```

**Response:**
```json
[
  {
    "userId": "guid",
    "status": "online",
    "lastSeen": "2025-01-01T12:00:00Z",
    "currentChannel": "channel-id"
  }
]
```

#### Get User Presence
```http
GET /api/presence/{userId}
```

### SignalR Hub: `/presencehub`

#### Client → Server Methods

**SetOnline**
```javascript
connection.invoke("SetOnline", userId, channelId);
```

**SetOffline**
```javascript
connection.invoke("SetOffline", userId);
```

**NotifyTyping**
```javascript
connection.invoke("NotifyTyping", userId, channelId);
```

#### Server → Client Events

**UserOnline**
```javascript
connection.on("UserOnline", (userId) => {
  // Handle user coming online
});
```

**UserOffline**
```javascript
connection.on("UserOffline", (userId) => {
  // Handle user going offline
});
```

**UserTyping**
```javascript
connection.on("UserTyping", (userId, channelId) => {
  // Handle typing indicator
});
```

---

## Webhook API

### Endpoints

#### List Webhooks
```http
GET /api/webhooks
```

**Response:**
```json
[
  {
    "id": "guid",
    "url": "https://example.com/webhook",
    "event": "message.created",
    "secret": "***",
    "isActive": true,
    "createdAt": "2025-01-01T00:00:00Z"
  }
]
```

#### Register Webhook
```http
POST /api/webhooks
Content-Type: application/json

{
  "url": "https://example.com/webhook",
  "event": "message.created",
  "secret": "optional-secret-for-hmac"
}
```

#### Remove Webhook
```http
DELETE /api/webhooks/{id}
```

#### Trigger Webhook (Manual Testing)
```http
POST /api/webhooks/trigger
Content-Type: application/json

{
  "event": "message.created",
  "data": {
    "channelId": "world",
    "fromUser": "user-id",
    "content": "Hello!"
  }
}
```

### Webhook Payload Format

When a webhook is triggered, it receives:

```json
{
  "event": "message.created",
  "data": {
    // Event-specific data
  },
  "timestamp": "2025-01-01T12:00:00Z"
}
```

**Headers:**
- `Content-Type: application/json`
- `X-Webhook-Signature: <hmac-sha256-hex>` (if secret is configured)

### Supported Events
- `message.created` - When a new message is sent
- `user.joined` - When a user joins a channel
- Custom events can be added

---

## Archive API

### Endpoints

#### Archive Messages
```http
POST /api/archive
Content-Type: application/json

{
  "channelId": "guid",
  "messages": [
    {
      "id": "guid",
      "channelId": "guid",
      "userId": "guid",
      "content": "message text",
      "sentAt": "2025-01-01T12:00:00Z"
    }
  ]
}
```

**Response:**
```json
{
  "id": "guid",
  "channelId": "guid",
  "startDate": "2025-01-01T00:00:00Z",
  "endDate": "2025-01-07T23:59:59Z",
  "filePath": "/archives/channel_guid/2025-01-01_to_2025-01-07.ndjson.gz",
  "messageCount": 1000,
  "archivedAt": "2025-01-08T00:00:00Z"
}
```

#### Retrieve Archived Messages
```http
GET /api/archive/retrieve?channelId={guid}&startDate={iso-date}&endDate={iso-date}
```

**Response:**
```json
[
  {
    "id": "guid",
    "channelId": "guid",
    "userId": "guid",
    "content": "message text",
    "sentAt": "2025-01-01T12:00:00Z"
  }
]
```

#### Get Archive Metadata
```http
GET /api/archive/metadata?channelId={guid}
```

**Response:**
```json
[
  {
    "id": "guid",
    "channelId": "guid",
    "startDate": "2025-01-01T00:00:00Z",
    "endDate": "2025-01-07T23:59:59Z",
    "filePath": "/archives/channel_guid/2025-01-01_to_2025-01-07.ndjson.gz",
    "messageCount": 1000,
    "archivedAt": "2025-01-08T00:00:00Z"
  }
]
```

---

## Error Responses

All APIs return standard HTTP status codes:

- `200 OK` - Success
- `201 Created` - Resource created
- `204 No Content` - Success with no response body
- `400 Bad Request` - Invalid request
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server error

Error response format:
```json
{
  "error": "Error message description"
}
```

---

## Rate Limiting

Currently, no rate limiting is implemented. For production:
- Implement API rate limiting
- Add authentication
- Use API keys for webhook subscriptions

---

## CORS

All services are configured with CORS for local development:
- Allowed origins: `http://localhost:5000`, `https://localhost:5001`
- Allowed methods: All
- Allowed headers: All

For production, update CORS policies to restrict to actual domains.
