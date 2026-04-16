using System.Text.Json;
using ConsoleApp.Models;

namespace ConsoleApp;

public abstract class FileStorage(string Key)
{
    private string StorageLocation
    {
        get
        {
            string executableFolder = AppContext.BaseDirectory;
            return $"{executableFolder}{Key}.json";
        }
        
    }

    public bool Exists()
    {
        return File.Exists(StorageLocation);
    }

    public DateTime LastUpdated
    {
        get
        {
            var entry = GetEntryAsync<object>().Result;
            return entry?.UpdatedOn ?? DateTime.MinValue;
        }
    }
    
    protected async Task StoreAsync<T>(T value) where T : class
    {
        var entry = new StorageEntry<T>
        {
            Value = value,
            UpdatedOn = DateTime.UtcNow
        };

        string jsonString = JsonSerializer.Serialize(entry);

        await using var writer = new StreamWriter(StorageLocation);
        await writer.WriteAsync(jsonString);

        writer.Close();
    }
    
    protected async Task<StorageEntry<T>?> GetEntryAsync<T>() where T : class
    {
        if (!File.Exists(StorageLocation))
            return null;
        
        using var reader = new StreamReader(StorageLocation);
        string jsonContent = await reader.ReadToEndAsync();
        
        try
        {
            var result = JsonSerializer.Deserialize<StorageEntry<T>>(jsonContent);
            
            return result;
        }
        catch (Exception e)
        {
            return null;
        }
    }
}