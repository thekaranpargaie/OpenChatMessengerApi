using ChatUI.Components;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add HttpClient for API calls
builder.Services.AddHttpClient("UserApi", client =>
{
    client.BaseAddress = new Uri("http://user-api");
});

builder.Services.AddHttpClient("ChatApi", client =>
{
    client.BaseAddress = new Uri("http://chat-api");
});

builder.Services.AddHttpClient("PresenceApi", client =>
{
    client.BaseAddress = new Uri("http://presence-api");
});

var app = builder.Build();

app.MapDefaultEndpoints();

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
