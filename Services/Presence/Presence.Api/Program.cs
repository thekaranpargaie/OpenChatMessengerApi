using Presence.Api.Hubs;
using Presence.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<PresenceService>();
builder.Services.AddSignalR();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS - Allow Docker Compose service names and localhost
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
                "http://localhost:5000", 
                "https://localhost:5001",
                "http://openchat-chat-ui:8080",
                "http://chat-ui:8080")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseHttpsRedirection();

app.MapHub<PresenceHub>("/presencehub");

// Add API endpoint to get online users
app.MapGet("/api/presence/online", (PresenceService service) =>
{
    return service.GetOnlineUsers();
});

app.MapGet("/api/presence/{userId}", (string userId, PresenceService service) =>
{
    var presence = service.GetUserPresence(userId);
    return presence != null ? Results.Ok(presence) : Results.NotFound();
});

app.Run();

