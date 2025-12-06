using SpotifyAPI.Web;


namespace SpotifyAIRecommender.Services
{
    public class SpotifyApiService
    {
        private readonly SpotifyClient _client;


        public SpotifyApiService(string accessToken)
        {
            _client = new SpotifyClient(accessToken);
        }


        public async Task<List<string>> SearchTracksAsync(string query)
        {
            var result = await _client.Search.Item(new SearchRequest(SearchRequest.Types.Track, query));


            return result.Tracks.Items
                .Select(t => $"{t.Name} - {t.Artists.First().Name}")
                .ToList();
        }


        public async Task<List<string>> GetRecommendationsAsync(string seedTrackId)
        {
            var rec = await _client.Recommendations.Get(new RecommendationsRequest
            {
                SeedTracks = new List<string> { seedTrackId },
                Limit = 10
            });


            return rec.Tracks.Select(t => $"{t.Name} - {t.Artists.First().Name}").ToList();
        }
    }
}