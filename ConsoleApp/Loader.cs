namespace ConsoleApp;

public class Loader
{
    public volatile bool _isRunning;
    public Task? _animationTask;
    
    public void Start(string message = "Loading")
    {
        if (_isRunning) return; // already running

        _isRunning = true;
        _animationTask = Task.Run(() =>
        {
            int dashCount = 1;

            while (_isRunning)
            {
                Console.Write($"\r{message} {new string('-', dashCount)}   ");
                dashCount = (dashCount % 5) + 1; // 1..5 dashes
                Thread.Sleep(200);
            }
        });
    }
    
    public void Stop(string doneMessage = "Done")
    {
        if (!_isRunning) return;

        _isRunning = false;
        _animationTask?.Wait();

        // Clear line and print final message
        Console.Write("\r" + new string(' ', Console.WindowWidth - 1) + "\r");
        Console.WriteLine(doneMessage);
    }
    
}