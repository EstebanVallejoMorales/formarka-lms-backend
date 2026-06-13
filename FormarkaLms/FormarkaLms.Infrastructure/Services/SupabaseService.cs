using FormarkaLms.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Supabase;

namespace FormarkaLms.Infrastructure.Services;

public class SupabaseService : ISupabaseService
{
    private readonly Client _supabaseClient;
    private readonly string? _serviceRoleKey;

    public SupabaseService(IConfiguration configuration)
    {
        var url = configuration["Supabase:ProjectUrl"];
        _serviceRoleKey = configuration["Supabase:SecretKey"]; // Replaces legacy service_role key

        if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(_serviceRoleKey))
        {
            throw new ArgumentException("Supabase URL and Secret Key must be configured.");
        }

        var options = new SupabaseOptions
        {
            AutoConnectRealtime = false,
            AutoRefreshToken = false
        };

        _supabaseClient = new Client(url, _serviceRoleKey, options);
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        try
        {
            var url = _supabaseClient.Auth.Options.Url;
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("apikey", _serviceRoleKey);
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _serviceRoleKey);

            var response = await httpClient.DeleteAsync($"{url}/admin/users/{userId}");
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting user from Supabase via API: {ex.Message}");
            return false;
        }
    }
}
