namespace ConsoleApp.Abstract;

public interface IOllamaService : IDisposable
{
    public Task StartAsync();
    public Task<string> SendPromptAsync(string prompt);
    public Task StreamPromptAsync(string prompt, Action<string> onTokenReceived);
}