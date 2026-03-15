using ConsoleApp.Credentials;
using SpotifyAPI.Web;

public class SpotifyApiService
{
    public async Task<List<string>> SearchTracksAsync(string query)
    {
        var accessToken = await CredentialStorage.GetTokenAsync("spotify");
        var _client = new SpotifyClient(accessToken.access_token, accessToken.token_type);

        try
        {
            var search = await _client.Search.Item(new SearchRequest(SearchRequest.Types.Track, query));
            
            return search.Tracks.Items
                .Select(t => $"{t.Name} - {t.Artists[0].Name}")
                .ToList();

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}