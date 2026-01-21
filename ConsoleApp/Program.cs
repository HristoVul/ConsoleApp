using SpotifyAIRecommender.Services;
using System;
using System.Threading.Tasks;
using System.Text;

class Program
{
    static async Task Main()
    {  
        {Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
        }
        Console.WriteLine("Spotify + AI Music Recommendation System");

        var auth = new SpotifyOAuthService();
        string token = await auth.AuthenticateAsync();

        if (string.IsNullOrEmpty(token))
        {
            Console.WriteLine("OAuth failed – token is empty");
            return;
        }

        var spotify = new SpotifyApiService(token);

        Console.Write("Enter artist or song: ");
        string query = Console.ReadLine();

        var tracks = await spotify.SearchTracksAsync(query);

        Console.WriteLine("Find songs:");
        foreach (var t in tracks)
        {
            Console.WriteLine(" - " + t);
        }

        var openAiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        if (string.IsNullOrEmpty(openAiKey))
        {
            Console.WriteLine("OPENAI_API_KEY not found");
            return;
        }

        var ai = new AiService(openAiKey);
        var result = await ai.AskAsync("give me 10 most popular tracks");

        Console.WriteLine();
        Console.WriteLine("AI Рекомендации:");
        Console.WriteLine(result);
    }
}