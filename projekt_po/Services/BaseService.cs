using projekt_po.Utils;

namespace projekt_po.Services;

// base service class for all services
public abstract class BaseService
{
    protected event LogEventHandler LogEvent;

    protected BaseService(ILogger logger)
    {
        LogEvent += logger.Log;
    }

    protected void Log(string message)
    {
        LogEvent.Invoke(message);
    }
}