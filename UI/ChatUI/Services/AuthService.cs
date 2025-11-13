using ChatUI.Models;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace ChatUI.Services;

public class AuthService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IJSRuntime _jsRuntime;
    private UserData? _currentUser;
    private string? _token;

    public event Action? OnAuthStateChanged;

    public AuthService(IHttpClientFactory httpClientFactory, IJSRuntime jsRuntime)
    {
        _httpClientFactory = httpClientFactory;
        _jsRuntime = jsRuntime;
    }

    public UserData? CurrentUser => _currentUser;
    public string? Token => _token;
    public bool IsAuthenticated => _currentUser != null;

    public async Task<(bool success, string message)> LoginAsync(LoginRequest request)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("UserApi");
            var response = await client.PostAsJsonAsync("api/v1/Auth/login", request);

            if (response.IsSuccessStatusCode)
            {
                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (loginResponse?.Success == true)
                {
                    _token = loginResponse.Token;
                    _currentUser = loginResponse.UserData;
                    
                    // Store token in localStorage
                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", _token);
                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "currentUser", System.Text.Json.JsonSerializer.Serialize(_currentUser));
                    
                    OnAuthStateChanged?.Invoke();
                    return (true, "Login successful");
                }
            }

            return (false, "Invalid email or password");
        }
        catch (Exception ex)
        {
            return (false, $"Login failed: {ex.Message}");
        }
    }

    public Task<(bool success, string message)> RegisterAsync(RegisterRequest request)
    {
        // For now, redirect to login since backend registration endpoint may not be implemented
        // This can be updated when the backend endpoint is ready
        return Task.FromResult<(bool, string)>((false, "Registration endpoint not yet implemented. Please contact administrator."));
    }

    public async Task LogoutAsync()
    {
        _currentUser = null;
        _token = null;
        
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "currentUser");
        
        OnAuthStateChanged?.Invoke();
    }

    public async Task<bool> CheckAuthStateAsync()
    {
        try
        {
            var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
            var userJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "currentUser");

            if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(userJson))
            {
                _token = token;
                _currentUser = System.Text.Json.JsonSerializer.Deserialize<UserData>(userJson);
                OnAuthStateChanged?.Invoke();
                return true;
            }
        }
        catch
        {
            // If there's an error reading from localStorage, just ignore it
        }

        return false;
    }
}
