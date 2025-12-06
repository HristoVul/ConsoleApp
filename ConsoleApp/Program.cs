using SpotifyAIRecommender.Services;


class Program
{
    static async Task Main()
    {
        Console.WriteLine("Spotify + AI Music Recommendation System");


        var auth = new SpotifyOAuthService();
        string token = await auth.AuthenticateAsync();


        var spotify = new SpotifyApiService(token);


        Console.Write("Въведи изпълнител/песен: ");
        string query = Console.ReadLine();


        var tracks = await spotify.SearchTracksAsync(query);


        Console.WriteLine("Намерени песни:");
        foreach (var t in tracks)
            Console.WriteLine(" - " + t);


        var ai = new AiService("YOUR_OPENAI_KEY");
        var result = ai.GenerateAIMusicRecommendations(tracks);


        Console.WriteLine("\nAI Рекомендации:");
        Console.WriteLine(result);
    }
}