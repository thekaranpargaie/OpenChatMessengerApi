using ChatUI.Components;
using ChatUI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add HttpClient for API calls
builder.Services.AddHttpClient("UserApi", client =>
{
    client.BaseAddress = new Uri("http://user-api:8080");
});

builder.Services.AddHttpClient("ChatApi", client =>
{
    client.BaseAddress = new Uri("http://chat-api:8080");
});

builder.Services.AddHttpClient("PresenceApi", client =>
{
    client.BaseAddress = new Uri("http://presence-api:8080");
});

// Add AuthService as scoped
builder.Services.AddScoped<AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
