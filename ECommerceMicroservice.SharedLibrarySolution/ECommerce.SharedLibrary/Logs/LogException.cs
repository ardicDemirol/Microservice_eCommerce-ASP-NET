using Serilog;

namespace ECommerce.SharedLibrary.Logs;

public static class LogException
{
    public static void LogExceptions(Exception ex)
    {
        LogToFile(ex.Message);
        LogToConsole(ex.Message);
        LogToDebug(ex.Message);
    }


    public static void LogToFile(string message)
    {
        Log.Information(message);
    }

    public static void LogToConsole(string message)
    {
        Log.Warning(message);
    }
    public static void LogToDebug(string message)
    {
        Log.Debug(message);
    }

}
