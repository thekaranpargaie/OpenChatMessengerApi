// Theme Management
window.themeManager = {
    init: function() {
        const savedTheme = localStorage.getItem('theme') || 'light';
        this.setTheme(savedTheme);
    },
    
    setTheme: function(theme) {
        document.documentElement.setAttribute('data-theme', theme);
        localStorage.setItem('theme', theme);
    },
    
    toggleTheme: function() {
        const currentTheme = document.documentElement.getAttribute('data-theme') || 'light';
        const newTheme = currentTheme === 'light' ? 'dark' : 'light';
        this.setTheme(newTheme);
        return newTheme;
    },
    
    getCurrentTheme: function() {
        return document.documentElement.getAttribute('data-theme') || 'light';
    }
};

// Audio Notifications
window.audioManager = {
    sounds: {},
    
    init: function() {
        // Create audio elements for notifications
        this.sounds.messageSent = this.createBeep(800, 0.1, 'sine');
        this.sounds.messageReceived = this.createBeep(600, 0.15, 'sine');
        this.sounds.notification = this.createBeep(1000, 0.2, 'triangle');
    },
    
    createBeep: function(frequency, duration, type = 'sine') {
        return function() {
            try {
                const audioContext = new (window.AudioContext || window.webkitAudioContext)();
                const oscillator = audioContext.createOscillator();
                const gainNode = audioContext.createGain();
                
                oscillator.connect(gainNode);
                gainNode.connect(audioContext.destination);
                
                oscillator.frequency.value = frequency;
                oscillator.type = type;
                
                gainNode.gain.setValueAtTime(0.3, audioContext.currentTime);
                gainNode.gain.exponentialRampToValueAtTime(0.01, audioContext.currentTime + duration);
                
                oscillator.start(audioContext.currentTime);
                oscillator.stop(audioContext.currentTime + duration);
            } catch (error) {
                console.error('Audio playback failed:', error);
            }
        };
    },
    
    playMessageSent: function() {
        if (this.sounds.messageSent) {
            this.sounds.messageSent();
        }
    },
    
    playMessageReceived: function() {
        if (this.sounds.messageReceived) {
            this.sounds.messageReceived();
        }
    },
    
    playNotification: function() {
        if (this.sounds.notification) {
            this.sounds.notification();
        }
    }
};

// Browser Notifications
window.notificationManager = {
    permission: 'default',
    
    init: async function() {
        if ('Notification' in window) {
            this.permission = Notification.permission;
            if (this.permission === 'default') {
                this.permission = await Notification.requestPermission();
            }
        }
    },
    
    show: function(title, options = {}) {
        if (this.permission === 'granted' && 'Notification' in window) {
            const notification = new Notification(title, {
                icon: '/favicon.png',
                badge: '/favicon.png',
                ...options
            });
            
            notification.onclick = function() {
                window.focus();
                notification.close();
            };
            
            setTimeout(() => notification.close(), 5000);
        }
    },
    
    showMessage: function(senderName, message) {
        this.show('New Message', {
            body: `${senderName}: ${message}`,
            tag: 'message-notification'
        });
    }
};

// Initialize on load
document.addEventListener('DOMContentLoaded', function() {
    window.themeManager.init();
    window.audioManager.init();
    window.notificationManager.init();
});

// Auto-scroll to bottom helper
window.scrollToBottom = function(elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        element.scrollTop = element.scrollHeight;
    }
};

// Format time helper
window.formatTime = function(date) {
    const d = new Date(date);
    const now = new Date();
    const diff = now - d;
    
    // Less than 1 minute
    if (diff < 60000) {
        return 'Just now';
    }
    
    // Less than 1 hour
    if (diff < 3600000) {
        const minutes = Math.floor(diff / 60000);
        return `${minutes}m ago`;
    }
    
    // Less than 1 day
    if (diff < 86400000) {
        const hours = Math.floor(diff / 3600000);
        return `${hours}h ago`;
    }
    
    // Format as time if today
    if (d.toDateString() === now.toDateString()) {
        return d.toLocaleTimeString('en-US', { hour: 'numeric', minute: '2-digit' });
    }
    
    // Format as date if not today
    return d.toLocaleDateString('en-US', { month: 'short', day: 'numeric' });
};
