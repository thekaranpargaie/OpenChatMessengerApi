# Modern Chat UI - User Guide

## Overview

The OpenChatMessenger now features a modern, responsive chat interface built with Blazor Server, inspired by Facebook Messenger's design principles.

## Features

### 🎨 Modern Design
- **Clean Interface**: Minimal, distraction-free design focused on conversations
- **Smooth Animations**: Subtle transitions and animations for a polished feel
- **Responsive Layout**: Seamless experience across desktop, tablet, and mobile devices
- **Dark/Light Themes**: Toggle between themes with persistent preference storage

### 🔐 Authentication
- **Secure Login**: JWT-based authentication with token storage in localStorage
- **User Registration**: Registration page UI ready (backend integration pending)
- **Session Persistence**: Stay logged in across browser sessions
- **Profile Display**: User avatar and status indicators

### 💬 Chat Features
- **Real-time Messaging**: Instant message delivery via SignalR
- **Typing Indicators**: See when someone is typing
- **Message Timestamps**: Clear time indicators for all messages
- **Channel Navigation**: Quick search and selection of chat channels
- **User Presence**: Online/offline status indicators

### 🔔 Notifications
- **Audio Feedback**: Pleasant sound effects when sending/receiving messages
- **Browser Notifications**: Get notified even when the tab is not active
- **Visual Badges**: Unread message counts (backend integration needed)

### ⚡ Performance
- **Auto-reconnection**: SignalR automatically reconnects if connection is lost
- **Optimized Rendering**: Efficient updates using Blazor Server
- **Lazy Loading**: Messages and channels load on demand

## Getting Started

### 1. Access the Application

Navigate to `http://localhost:5000` when running with Docker Compose.

### 2. Sign In

Click "Sign In" on the home page and enter your credentials:
- Email: Your registered email address
- Password: Your password

**Note**: Make sure you have a user account created via the User API.

### 3. Start Chatting

1. **Select a Channel**: Click on any channel in the sidebar
2. **Send Messages**: Type your message and press Enter or click the send button
3. **Toggle Theme**: Click the moon/sun icon to switch between dark and light modes

## UI Components

### Home Page
- Landing page with feature showcase
- Quick access to Sign In/Sign Up
- Displays user status if already logged in

### Login Page
- Modern gradient background
- Email and password fields
- Error message display
- Link to registration page

### Register Page
- Multi-field registration form
- Password confirmation
- UI complete (backend integration pending)

### Chat Interface

#### Sidebar (Left Panel)
- **User Profile**: Shows your avatar, name, and online status
- **Search Box**: Filter channels by name
- **Channel List**: All available channels with timestamps
- **Theme Toggle**: Switch between dark and light modes
- **Logout Button**: Sign out of the application

#### Main Chat Area (Right Panel)
- **Chat Header**: Selected channel name and member count
- **Messages Area**: Scrollable message history
- **Message Input**: Text area with send button
- **Typing Indicator**: Shows when others are typing

## Keyboard Shortcuts

- **Enter**: Send message
- **Shift + Enter**: New line in message (not yet implemented)

## Theme System

The application supports two themes:

### Light Theme
- White backgrounds
- Dark text
- Blue accents
- Soft shadows

### Dark Theme
- Dark backgrounds
- Light text
- Blue accents
- Deeper shadows

**Theme preference is saved** and persists across sessions.

## Audio Notifications

The app plays subtle sound effects for:
- **Message Sent**: Short high-pitched beep
- **Message Received**: Slightly lower-pitched beep
- **Notifications**: Triangle wave sound

Audio is generated using the Web Audio API - no audio files needed!

## Browser Notifications

When enabled, you'll receive browser notifications for:
- New messages (when tab is not focused)
- Channel invitations (future feature)

The app requests notification permission on first load.

## Mobile Experience

The UI is fully responsive and adapts to mobile screens:
- **Sidebar**: Slides in from the left on mobile
- **Touch-Friendly**: Larger tap targets for buttons
- **Optimized Layout**: Stacked elements for narrow screens
- **Gestures**: Swipe to close sidebar (future feature)

## Technical Details

### Built With
- **Blazor Server**: .NET 8 interactive components
- **SignalR**: Real-time bidirectional communication
- **CSS Custom Properties**: Easy theming via CSS variables
- **Web Audio API**: Programmatic sound generation
- **Notifications API**: Browser push notifications
- **LocalStorage API**: Client-side state persistence

### Architecture
```
┌─────────────────────────────────────┐
│           Browser (UI)              │
│  ┌──────────────┐  ┌──────────────┐│
│  │ Razor Pages  │  │   Services   ││
│  │              │  │              ││
│  │ - Login      │  │ - AuthService││
│  │ - Register   │  │              ││
│  │ - Chat       │  │              ││
│  │ - Home       │  │              ││
│  └──────────────┘  └──────────────┘│
└─────────────────────────────────────┘
         │           │
         │ HTTP      │ SignalR WebSocket
         ▼           ▼
┌─────────────────────────────────────┐
│          Backend Services           │
│  ┌──────────────┐  ┌──────────────┐│
│  │  User API    │  │  Chat API    ││
│  │  :5001       │  │  :5002       ││
│  │              │  │              ││
│  │ - Auth       │  │ - Channels   ││
│  │ - Profile    │  │ - Messages   ││
│  │              │  │ - ChatHub    ││
│  └──────────────┘  └──────────────┘│
└─────────────────────────────────────┘
```

## Customization

### Colors
Edit `/wwwroot/css/modern-chat.css` and modify the CSS custom properties:

```css
:root {
    --primary-color: #0084ff;    /* Main brand color */
    --secondary-color: #44bec7;  /* Accent color */
    --success-color: #00d084;    /* Success states */
    --danger-color: #fa383e;     /* Error states */
    /* ... more variables ... */
}
```

### Sounds
Modify sound frequencies in `/wwwroot/js/chat-app.js`:

```javascript
// In audioManager.init()
this.sounds.messageSent = this.createBeep(800, 0.1, 'sine');
//                                         ↑    ↑     ↑
//                                      frequency duration waveform
```

## Troubleshooting

### Can't Login
- Verify the User API is running on port 5001
- Check that you have a valid user account
- Clear browser cache and localStorage
- Check browser console for errors

### Messages Not Sending
- Ensure Chat API is running on port 5002
- Verify SignalR connection (check console)
- Check that you've selected a channel
- Verify you're authenticated

### No Audio
- Check browser audio permissions
- Ensure device is not muted
- Some browsers require user interaction before playing audio

### Theme Not Saving
- Check localStorage is enabled in browser
- Clear browser data and try again
- Check browser console for errors

### Browser Notifications Not Working
- Grant notification permissions when prompted
- Check browser notification settings
- Some browsers block notifications in certain modes

## Future Enhancements

Planned features for upcoming releases:
- [ ] Direct messaging (1:1 chats)
- [ ] Private channels with invite system
- [ ] Read receipts
- [ ] Message reactions/emoji
- [ ] File sharing
- [ ] Voice messages
- [ ] Video calls
- [ ] User search
- [ ] Message editing and deletion
- [ ] Channel management UI
- [ ] User settings panel
- [ ] Multiple language support

## Support

For issues or questions:
1. Check the troubleshooting section above
2. Review the application logs in browser console
3. Open an issue on GitHub

## License

See LICENSE.txt for details.
