namespace ConsoleApp.Ai;

public class KeywordExtractionAgent
{
    private readonly OllamaClient _client;

    public KeywordExtractionAgent()
    {
        _client = new OllamaClient();
    }

    public async Task<string> ExtractKeywords(string text)
    {
        var prompt = $@"
Extract 5-10 music-related keywords.
Return only comma-separated keywords.

{text}
";

        return await _client.GenerateAsync(prompt);
    }
}