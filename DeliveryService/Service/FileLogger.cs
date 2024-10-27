namespace DeliveryService.Service;

public class FileLogger
{
    private string _logFilePath;
    private static readonly string _defaultLogFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"ConfigurationFiles\default_log.txt");

    public FileLogger(string logFilePath)
    {
        _logFilePath = string.IsNullOrEmpty(logFilePath) ? _defaultLogFilePath : logFilePath;
        LogMessage("Logger initialized.");
    }

    private void Log(string message)
    {
        if (!File.Exists(_logFilePath))
        {
             File.WriteAllText(_logFilePath, string.Empty);
        }

        Console.WriteLine(message);

        File.AppendAllText(_logFilePath, $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:{message}{Environment.NewLine}");
    }

    public void LogMessage(string message)
    {
        try
        {
            if (string.IsNullOrEmpty(message))
            {
                message = "Logging error: default used because the input was null or empty";
            }

            Log(message);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
