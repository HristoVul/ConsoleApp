using OpenAI.Chat;


namespace SpotifyAIRecommender.Services
{
    public class AiService
    {
        private readonly ChatClient _client;


        public AiService(string apiKey)
        {
            _client = new ChatClient("gpt-4.1", apiKey);
        }


        public string GenerateAIMusicRecommendations(List<string> tracks)
        {
            string prompt = $"Предложи подобни песни: {string.Join(", ", tracks)}. Дай само изпълнител и заглавие.";


            var response = _client.Complete(prompt);
            return response;
        }
    }
}