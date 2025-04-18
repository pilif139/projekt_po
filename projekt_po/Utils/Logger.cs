namespace projekt_po.Utils;

/// <summary>
/// Delegate for handling log events.
/// </summary>
/// <param name="message">The log message.</param>
public delegate void LogEventHandler(string message);

/// <summary>
/// Interface for logging functionality.
/// </summary>
public interface ILogger
{
    /// <summary>
    /// Logs a message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void Log(string message);

    /// <summary>
    /// Cleans the log file.
    /// </summary>
    void Clean();
}

/// <summary>
/// Implementation of a file-based logger.
/// </summary>
public class Logger : ILogger
{
    private readonly string _filePath;

    /// <summary>
    /// Initializes a new instance of the <see cref="Logger"/> class.
    /// </summary>
    /// <param name="filePath">The path to the log file.</param>
    public Logger(string filePath)
    {
        _filePath = filePath;
    }
    
    /// <summary>
    /// Logs a message to the file.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void Log(string message)
    {
        using var writer = new StreamWriter(_filePath, true);
        writer.WriteLine($"{DateTime.Now} - {message}");
    }
    
    /// <summary>
    /// Cleans the log file by overwriting it.
    /// </summary>
    public void Clean()
    {
        using var writer = new StreamWriter(_filePath, false);
    }
}
