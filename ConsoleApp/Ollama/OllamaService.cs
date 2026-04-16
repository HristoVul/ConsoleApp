using System.Diagnostics;
using System.Text;
using System.Text.Json;
using ConsoleApp.Abstract;

namespace ConsoleApp;

public class OllamaService : IOllamaService
{
    private readonly string _model;
    private Process? _ollamaProcess;
    private readonly HttpClient _http = new HttpClient(){ Timeout = TimeSpan.FromMinutes(10)};
    private bool _disposed;

    public OllamaService(string model)
    {
        this._model = model;
    }
    
    public async Task StartAsync()
    {
        StartServer();
        Console.WriteLine("Starting Ollama server...");
        
        await WaitForServerAsync();
        Console.WriteLine("Ollama server started.");

        Console.WriteLine($"Ensuring model '{_model}' is installed...");
        await PullModelAsync();
        
        Console.WriteLine($"Warming up model '{_model}'...");
        await QueryModelAsync("Hello");
        
        Console.WriteLine($"Ollama started with model {_model}");
    }
    
    public async Task<string> SendPromptAsync(string prompt)
    {
        return await QueryModelAsync(prompt);
    }
    
    public async Task StreamPromptAsync(string prompt, Action<string> onTokenReceived)
    {
        var body = new
        {
            model = _model,
            prompt = prompt,
            stream = true
        };

        string json = JsonSerializer.Serialize(body);

        var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:11434/api/generate")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        using var response = await _http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

        using var stream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line))
                continue;

            try
            {
                using var doc = JsonDocument.Parse(line);
                if (doc.RootElement.TryGetProperty("response", out var tokenElement))
                {
                    string token = tokenElement.GetString();
                    onTokenReceived(token);
                }
            }
            catch
            {
                // ignore malformed chunks
            }
        }
    }
    
    // ---------------------------------------------------------
    // START WORKFLOW
    // ---------------------------------------------------------
    private void StartServer()
    {
        if (_ollamaProcess != null && !_ollamaProcess.HasExited)
            return;

        var psi = NewOllamaProcessInfo("serve");

        _ollamaProcess = Process.Start(psi);
    }
    
    private async Task WaitForServerAsync()
    {
        using var http = new HttpClient();
        while (true)
        {
            try
            {
                var res = await http.GetAsync("http://localhost:11434/api/tags");
                if (res.IsSuccessStatusCode)
                    return;
            }
            catch
            {
                // ignored
            }

            await Task.Delay(500);
        }
    }

    private async Task PullModelAsync()
    {
        var processInfo = NewOllamaProcessInfo($"pull {this._model}");
        
        processInfo.RedirectStandardOutput = true;
        processInfo.RedirectStandardError = true;
        
        var process = Process.Start(processInfo);
        await process.WaitForExitAsync();
    }
    
    private async Task<string> QueryModelAsync(string prompt)
    {
        var body = new
        {
            model = _model,
            prompt = prompt,
            stream = false
        };

        string json = JsonSerializer.Serialize(body);

        var response = await _http.PostAsync(
            "http://localhost:11434/api/generate",
            new StringContent(json, Encoding.UTF8, "application/json")
        );

        string resultJson = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(resultJson);
        return doc.RootElement.GetProperty("response").GetString();
    }
    
    // ---------------------------------------------------------
    // STOP WORKFLOW
    // ---------------------------------------------------------
    private void UnloadModel()
    {
        try
        {
            var psi = NewOllamaProcessInfo($"stop {_model}");
            
            Process.Start(psi);
            Console.WriteLine($"Unloading model '{_model}'...");
        }
        catch
        {
            Console.WriteLine("Failed to unload model.");
        }
    }
    
    private void StopServer()
    {
        try
        {
            if (_ollamaProcess != null && !_ollamaProcess.HasExited)
            {
                Console.WriteLine("Stopping Ollama server...");
                _ollamaProcess.Kill(true);
                _ollamaProcess.WaitForExit(3000);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to stop Ollama server: {ex.Message}");
        }
    }
    
    // ---------------------------------------------------------
    // HELPERS
    // ---------------------------------------------------------
    
    private ProcessStartInfo NewOllamaProcessInfo(string arguments = "")
    {
        return new ProcessStartInfo
        {
            FileName = "ollama",
            Arguments = arguments,
            UseShellExecute = false,
            CreateNoWindow = true
        };
    }

    public void Dispose()
    {
        if (_disposed) return;

        UnloadModel();
        StopServer();
        _http.Dispose();

        _disposed = true;
    }
}