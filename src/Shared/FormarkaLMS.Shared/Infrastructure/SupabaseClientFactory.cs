using Supabase;

namespace FormarkaLMS.Shared.Infrastructure;

public class SupabaseSettings
{
    public string Url { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
}

public static class SupabaseClientFactory
{
    public static Client CreateClient(string url, string key)
    {
        var options = new SupabaseOptions
        {
            AutoConnectRealtime = true
        };
        return new Client(url, key, options);
    }
}
