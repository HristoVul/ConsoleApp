using System.Text;
using ConsoleApp.Models;
using ConsoleApp.Ollama;
using ConsoleApp.Storage;
using SpotifyAPI.Web;

namespace ConsoleApp
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            
            var authService = new SpotifyOAuthService();
            var credentials = new CredentialStorage();

            SpotifyClient client;
            if (await credentials.HasValidTokenAsync())
            {
                var token = await credentials.GetTokenAsync();
                client = await authService.FromAccessToken(token!);
            }
            else
            {
                client = await authService.AuthenticateAsync();
            }

            await credentials.SaveTokenAsync(authService.CurrentAccess);

            var spotifyApi = new SpotifyApiService(client);

            PrivateUser profile = await spotifyApi.GetUserProfileAsync();
        
            var profileStore = new ProfileStorage(profile.Id);

            if (!profileStore.Exists() || profileStore.LastUpdated < DateTime.Now.AddDays(-10))
            {
                List<FullArtist> topArtists = await spotifyApi.GetTopArtistsAsync();
                List<FullTrack> topTracks = await spotifyApi.GetTopTracksAsync();
                List<PlayHistoryItem> recentHistory = await spotifyApi.GetRecentHistory();

                ProfileData profileData = new ProfileData
                {
                    Artists = topArtists.Select(t => t.Name).ToArray(),
                
                    Tracks = topTracks.Select(t => new TrackEntry(t.Name, t.Artists.First().Name, t.Album.Name, t.Href))
                        .ToList(),
                    PlaybackHistory = recentHistory.Select(h =>
                        new TrackEntry(h.Track.Name, h.Track.Artists.First().Name, h.Track.Album.Name, h.Track.Href)).ToList()
                };

                await profileStore.SaveKeywordReferenceAsync(profileData.ToKeywordReference());
            }

            // var ollamaService = new OllamaService("phi3:latest");
            var ollamaService = new OllamaService("gemma4:e2b");
            await ollamaService.StartAsync();
        
            var ollamaClient = new OllamaClient(ollamaService, await profileStore.GetKeywordReferenceAsync());

            Console.WriteLine($"Hey, {profile.DisplayName}, what's on your mind:");

            var loader = new Loader();
        
            while (true)
            {
                var input = Console.ReadLine() ?? "";
                if (input.ToLower() == "exit")
                {
                    ollamaClient.Dispose();   
                    break;
                }

                loader.Start("Getting keywords");
                var keywords = await ollamaClient.ExtractKeywords(input);
                
                loader.Stop("\nKeywords:");
                Console.WriteLine(keywords);

                await ollamaClient.StreamRecommendationsAsync(input, keywords);

                Console.WriteLine();
            }
        }
    }
}