using projekt_po.Utils;

namespace projekt_po.Services;

// base service class for all services
public abstract class BaseService
{
    // event that will be used to log messages
    protected event LogEventHandler LogEvent;

    protected BaseService(ILogger logger)
    {
        // adds logger to the event
        LogEvent += logger.Log;
    }

    protected void Log(string message)
    {
        // invokes the event to log the message with logger class instance
        LogEvent.Invoke(message);
    }
}