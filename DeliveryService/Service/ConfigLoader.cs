using DeliveryService.Models;
using Newtonsoft.Json;

namespace DeliveryService.Service;

public static class ConfigLoader
{
    public static FileConfig LoadConfig(string configFilePath, FileLogger logger)
    {
        if (string.IsNullOrWhiteSpace(configFilePath))
        {
            logger.LogMessage($"Configuration file {configFilePath} is empty.");
            throw new ArgumentNullException();
        }
        try
        {
            if (!File.Exists(configFilePath))
            {
                logger.LogMessage($"Configuration file {configFilePath} not found.");
                throw new NullReferenceException();
            }

            var pathConfig = File.ReadAllText(configFilePath);
            if(string.IsNullOrEmpty(pathConfig))
            {
                logger.LogMessage($"Configuration file {pathConfig} is empty.");
                throw new NullReferenceException($"Configuration file {pathConfig} is empty.");
            }

            var resultConfig = JsonConvert.DeserializeObject<FileConfig>(pathConfig);

            if (resultConfig == null)
            {
                logger.LogMessage($"Failed to deserialize configuration {resultConfig}");
                throw new NullReferenceException($"Failed to deserialize configuration {resultConfig}");
            }

            return resultConfig;
        }
        catch (IOException ioEx)
        {
            logger.LogMessage($"File I/O error: {ioEx.Message}");
            throw;
        }
        catch (JsonReaderException jsonEx)
        {
            logger.LogMessage($"JSON parsing error: {jsonEx.Message}");
            throw;
        }
        catch (Exception ex)
        {
            logger.LogMessage($"Unexpected error: {ex.Message}");
            throw;
        }
    }
}
