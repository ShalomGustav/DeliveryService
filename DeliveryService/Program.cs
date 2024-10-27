using DeliveryService.Service;
using DeliveryService.Models;

public class Program
{
    public static string globalConfigPath = Path.Combine(Directory.GetCurrentDirectory(), @"ConfigurationFiles\config.json");
    public static FileConfig fileConfig = new FileConfig(); 
    private static void Main(string[] args)
    {
        try
        {
            int indexRegion;
            string separator;

            var logger = new FileLogger(Path.Combine(Directory.GetCurrentDirectory(), @"ConfigurationFiles\log.txt"));
            logger.LogMessage("Program started...");

            if (args.Length >= 4)
            {
                logger.LogMessage("Reading parameters from command line...");

                fileConfig.DeliveryOrders = args[0];

                if (!int.TryParse(args[1], out indexRegion))
                {
                    logger.LogMessage("Invalid city district. Please provide a valid integer value.");
                    return;
                }
                fileConfig.IndexRegion = indexRegion;

                fileConfig.FirstDeliveryTime = args[2];
                if (!DateTime.TryParseExact(fileConfig.FirstDeliveryTime, "yyyy-MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out _))
                {
                    logger.LogMessage($"Invalid date format: {fileConfig.FirstDeliveryTime}. Expected format: yyyy-MM-dd HH:mm:ss.");
                    return;
                }

                separator = args[3];
            }
            else
            {
                logger.LogMessage("Reading parameters from config file...");

                if (!File.Exists(globalConfigPath))
                {
                    logger.LogMessage($"Configuration file not found: {globalConfigPath}");
                    return;
                }

                try
                {
                    fileConfig = ConfigLoader.LoadConfig(globalConfigPath, logger);
                }
                catch
                {
                    Console.WriteLine("Configuration not load");
                }

                if (string.IsNullOrEmpty(fileConfig.DeliveryOrders))
                {
                    logger.LogMessage("Failed to load configuration.");
                    return;
                }

                if (!DateTime.TryParseExact(fileConfig.FirstDeliveryTime, "yyyy-MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out _))
                {
                    logger.LogMessage($"Invalid date format in configuration: {fileConfig.FirstDeliveryTime}. Expected format: yyyy-MM-dd HH:mm:ss.");
                    return;
                }

                separator = ",";
            }

            logger.LogMessage("Initializing order processing...");

            var orderService = new OrderService(logger, fileConfig, separator);
            orderService.ProcessOrders();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}
