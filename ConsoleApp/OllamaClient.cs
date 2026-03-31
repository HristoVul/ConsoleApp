using System.Text;
using System.Text.Json;

namespace ConsoleApp.Ai;

public class OllamaClient
{
    private readonly HttpClient _httpClient;

    public OllamaClient()
    {
        _httpClient = new HttpClient();
    }

    public async Task<string> GenerateAsync(string prompt)
    {
        var request = new
        {
            model = "phi3",
            prompt = prompt,
            stream = false
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("http://localhost:11434/api/generate", content);

        var responseString = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(responseString);

        var root = doc.RootElement;

        if (root.TryGetProperty("error", out var error))
        {
            return $"ERROR: {error.GetString()}";
        }

        if (root.TryGetProperty("response", out var result))
        {
            return result.GetString() ?? "";
        }

        return "No response from Ollama";
    }
}