using System.Text.Json;
using ConsoleApp.Models;

namespace ConsoleApp.Credentials;


public static class CredentialStorage
{
    public static async Task StoreAsync<T>(string key, T token) where T : class
    {
        var entry = new StorageEntry<T>
        {
            Value = token,
            UpdatedOn = DateTime.UtcNow
        };
        
        string jsonString = JsonSerializer.Serialize(entry);
        using (var writer = new StreamWriter(MakeFilePath(key)))
        {
            await writer.WriteAsync(jsonString);
            writer.Close();
        }
    }

    public static async Task<StorageEntry<T>?> GetEntryAsync<T>(string key) where T : class
    {
        using (var reader = new StreamReader(MakeFilePath(key)))
        {
            string jsonContent = await reader.ReadToEndAsync();

            var result = JsonSerializer.Deserialize<StorageEntry<T>>(jsonContent);
            
            return result;
        }
    }

    public static async Task<AccessToken> GetTokenAsync(string key)
    {
        var entry = await GetEntryAsync<AccessToken>(key);
        return entry?.Value;
    }
    
    public static async Task<bool> HasValidTokenAsync(string key)
    {
        if(!File.Exists(MakeFilePath(key))) 
            return false;
        
        var entry = await GetEntryAsync<AccessToken>(key);
        
        var secondsSinceUpdate = (DateTime.UtcNow - entry.UpdatedOn).TotalSeconds;

        return secondsSinceUpdate <= 3600;
    }
    
    private static string MakeFilePath(string fileName)
    {
        string executableFolder = AppContext.BaseDirectory;
        return $"{executableFolder}{fileName}.json";
    }
}