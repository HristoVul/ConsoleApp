using System.Collections.Generic;
using System.Threading.Tasks;
using OpenAI;
using OpenAI.Chat;

public class AiService
{
    private readonly OpenAIClient _client;

    public AiService(string apiKey)
    {
        _client = new OpenAIClient(apiKey);
    }

    public async Task<string> AskAsync(string prompt)
    {
        var chatClient = _client.GetChatClient("gpt-4o-mini");

        var messages = new List<ChatMessage>
        {
            new UserChatMessage(prompt)
        };

        var response = await chatClient.CompleteChatAsync(messages);

        return response.Value.Content[0].Text;
    }
}