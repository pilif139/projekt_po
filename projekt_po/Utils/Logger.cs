namespace projekt_po.Utils;

public delegate void LogEventHandler(string message);

public interface ILogger
{
    void Log(string message);
}

public class Logger : ILogger
{
    private readonly string _filePath;

    public Logger(string filePath)
    {
        _filePath = filePath;
    }
    
    public void Log(string message)
    {
        using var writer = new StreamWriter(_filePath, true);
        writer.WriteLine($"{DateTime.Now} - {message}");
    }
}