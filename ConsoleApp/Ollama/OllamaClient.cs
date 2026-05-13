using ConsoleApp.Abstract;

namespace ConsoleApp.Ollama;

public class OllamaClient : IDisposable
{
    private readonly IOllamaService _service;
    private readonly string _reference;

    public OllamaClient(IOllamaService service, string reference)
    {
        _service = service;
        _reference = reference;
    }

    public async Task<string> ExtractKeywords(string text)
    {
        var prompt = $@"
Extract 5-10 music-related keywords that best match the user's intent.

Requirements:
- Focus on the meaning and mood of the user's prompt.
- Reflect the user's taste using the listening profile reference.
- Prefer keywords related to the user's favorite artists, top tracks, recent playback history, and likely genres.
- Include both direct intent terms and relevant music descriptors when appropriate.
- Avoid generic or unrelated words.
- Return only comma-separated keywords.
- No explanations, no numbering, no bullets, no extra text.

User prompt:
{text}

Listening profile:
{_reference}
";

        return await _service.SendPromptAsync(prompt);
    }


    public async Task StreamRecommendationsAsync(string userPrompt, string keywords)
    {
        string prompt = $@"
Recommend songs based on the user input, extracted keywords, and listening profile.
Goals:
- Suggest songs that match the user's intent.
- Use the keywords array as the primary signal.
- Use the profile reference to personalize recommendations toward the user's taste.
- Prefer songs that are stylistically similar to the user's favorite artists, top tracks, and recent playback history.
- Do not repeat the same song or recommend obvious duplicates.

Output rules:
- Return exactly 10 to 15 songs.
- Return list of song titles or artist - song title pairs. Return each on a new line)
- Keep the recommendations concise and relevant.
- No explanations, no numbering, no bullets, no extra text.

User input:
{userPrompt}

Keywords:
{keywords}

Profile reference:
{_reference}
";

        await _service.StreamPromptAsync(prompt, Console.Write);
    }

    public void Dispose()
    {
        _service.Dispose();
    }
}