using ConsoleApp.Models;

namespace ConsoleApp.Storage;


public class CredentialStorage() : FileStorage("spotify")
{
    public async Task SaveTokenAsync(AccessToken token)
    {
        await StoreAsync(token);
    }
    
    public async Task<AccessToken?> GetTokenAsync()
    {
        var entry = await GetEntryAsync<AccessToken>();
        return entry?.Value ?? null;
    }

    public async Task<bool> HasValidTokenAsync()
    {
        var entry = await GetEntryAsync<AccessToken>();

        if (entry == null)
            return false;

        bool isExpired = entry.UpdatedOn.AddSeconds((double)entry.Value.ExpiresIn) <= DateTime.UtcNow;

        return !isExpired;
    }
}