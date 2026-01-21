using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpotifyAPI.Web;

public class SpotifyApiService
{
    private readonly SpotifyClient _client;

    public SpotifyApiService(string accessToken)
    {
        _client = new SpotifyClient(accessToken);
    }

    public async Task<List<string>> SearchTracksAsync(string query)
    {
        var search = await _client.Search.Item(new SearchRequest(SearchRequest.Types.Track, query));

        return search.Tracks.Items
            .Select(t => $"{t.Name} - {t.Artists[0].Name}")
            .ToList();
    }

    // public async Task<List<string>> GetRecommendationsAsync(string trackId)
    // {
    //     var request = new RecommendationsRequest
    //     {
    //         SeedTracks = new List<string> { trackId }
    //     };
    //
    //     var recs = await _client.Browse.GetRecommendations(request);
    //
    //     return recs.Tracks.Select(t => t.Name).ToList();
    // }
}