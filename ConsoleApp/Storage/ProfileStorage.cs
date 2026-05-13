namespace ConsoleApp.Storage;

public class ProfileStorage(string id) : FileStorage(id)
{
    public async Task SaveKeywordReferenceAsync(string keywordReference)
    {
        await StoreAsync(keywordReference);   
    }
    
    public async Task<string> GetKeywordReferenceAsync()
    {
        var entry = await GetEntryAsync<string>();
        return entry?.Value ?? String.Empty;
    }
}