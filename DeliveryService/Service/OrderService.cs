using DeliveryService.Models;
using Newtonsoft.Json;
using System.Globalization;

namespace DeliveryService.Service;

public class OrderService
{
    private FileConfig _fileConfig;
    private FileLogger _logger;
    private string _separator;

    public OrderService(FileLogger logger, FileConfig fileConfig, string separator)
    {
        _fileConfig = fileConfig;
        _logger = logger;
        _separator = separator;
    }

    public void ProcessOrders()
    {
        try
        {
            _logger.LogMessage("Starting order processing...");

            var orders = LoadOrders(_fileConfig.DeliveryOrders, _separator);

            if (orders.Count == 0)
            {
                _logger.LogMessage("No orders loaded from the file.");
                return;
            }
            _logger.LogMessage($"Loaded {orders.Count} orders from {_fileConfig.DeliveryOrders}");

            var firstDeliveryDateTime = DateTime.ParseExact(_fileConfig.FirstDeliveryTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            var filteredOrders = GetOrdersByFilter(_fileConfig.IndexRegion, firstDeliveryDateTime, orders);
            _logger.LogMessage($"Filtered {filteredOrders.Count} orders for region {_fileConfig.IndexRegion}");

            SaveFilteredOrdersToFile(filteredOrders, _fileConfig.ResultFilePath);
            _logger.LogMessage($"Filtered orders saved to {_fileConfig.ResultFilePath}");

            _logger.LogMessage("Order processing completed");
        }
        catch (Exception ex)
        {
            _logger.LogMessage($"Error: {ex.Message}");
        }
    }

    private List<Order> GetOrdersByFilter(int indexRegion, DateTime firstDeliveryDateTime, List<Order> orders)
    {
        if (indexRegion <= 0 || indexRegion.ToString().Length != 6)
        {
            _logger.LogMessage($"Error: Region index cannot be less than or equal to zero");
            throw new Exception($"Error: Region index cannot be less than or equal to zero");
        }

        if (firstDeliveryDateTime == DateTime.MinValue)
        {
            firstDeliveryDateTime = DateTime.Now;
            _logger.LogMessage($"Error: Invalid first delivery date time, using current " +
                $"date and time {firstDeliveryDateTime.ToString("yyyy-MM-dd HH:mm:ss")}");
        }

        try
        {
            var result = orders.Where(x => x.IndexRegion == indexRegion &&
                x.TimeDelivery >= firstDeliveryDateTime &&
                x.TimeDelivery <= firstDeliveryDateTime.AddMinutes(30)).ToList();
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogMessage(ex.Message);
            return new List<Order>();
        }
    }

    private List<Order> LoadOrders(string filePath, string separator)
    {
        var orders = new List<Order>();

        try
        {
            if (string.IsNullOrEmpty(filePath))
            {
                _logger.LogMessage("Path not is null or empty.");
            }

            if (!File.Exists(filePath))
            {
                _logger.LogMessage($"{filePath} is empty");
                throw new NullReferenceException($"{filePath} is empty.");
            }

            if (Path.GetExtension(filePath).ToLower() == ".txt")
            {
                return LoadOrdersOnTxt(filePath, separator);
            }

            return LoadOrdersOnJson(filePath);
        }
        catch (Exception ex)
        {
            _logger.LogMessage(ex.Message);
        }

        return orders;
    }

    private List<Order> LoadOrdersOnTxt(string filePath, string separator)
    {
        var orders = new List<Order>();
        var result = File.ReadAllLines(filePath);

        foreach (var line in result)
        {
            var parts = line.Split(separator);

            if (parts.Length != 4)
            {
                _logger.LogMessage($"Skipping line due to incorrect number of parts: {line}.");
                Console.WriteLine($"Skipping line due to incorrect number of parts: {line}.");
                continue;
            }

            try
            {
                var orderId = Guid.Parse(parts[0]);
                var weight = double.Parse(parts[1], CultureInfo.InvariantCulture);
                var indexRegion = int.Parse(parts[2]);
                var timeDelivery = DateTime.Parse(parts[3]);

                var order = new Order(orderId, weight, indexRegion, timeDelivery);

                if (!ValidateOrder(order))
                {
                    _logger.LogMessage($"Order {order} failed validation and was skipped.");
                    continue;
                }
                orders.Add(order);
            }
            catch(Exception ex)
            {
                _logger.LogMessage($"Unexpected error for line '{line}' in file '{filePath}': {ex.Message}.");
                continue;
            }
        }

        return orders;
    }

    private List<Order> LoadOrdersOnJson(string filePath)
    {
        var orders = new List<Order>();

        if (string.IsNullOrEmpty(filePath))
        {
            throw new ArgumentNullException(nameof(filePath));
        }

        try
        {
            var result = File.ReadAllText(filePath);
            var resultJson = JsonConvert.DeserializeObject<List<Order>>(result);

            foreach (var jsonItem in resultJson)
            {
                try
                {
                    if (!ValidateOrder(jsonItem))
                    {
                        _logger.LogMessage($"Order {jsonItem} failed validation and was skipped.");
                        continue;
                    }
                    orders.Add(jsonItem);
                }
                catch (JsonSerializationException ex)
                {
                    _logger.LogMessage($"Deserialization error for item in file '{filePath}': {ex.Message}");
                    continue; 
                }
            }
        }
        catch (JsonException ex)
        {
            _logger.LogMessage($"JSON parsing error for file '{filePath}': {ex.Message}");
            Console.WriteLine($"Error parsing JSON file '{filePath}': {ex.Message}");
        }

        return orders;
    }

    private void SaveFilteredOrdersToFile(List<Order> orders, string filePath)
    {
        if (orders == null || orders.Count == 0)
        {
            _logger.LogMessage("Error: Order list is null or empty. No data to save.");
            return;
        }

        if (string.IsNullOrEmpty(filePath))
        {
            _logger.LogMessage(filePath);
            return;
        }

        try
        {
            var lines = orders.Select(order => order.ToString()).ToArray();
            File.WriteAllLines(filePath, lines);
            _logger.LogMessage($"Successfully saved {orders.Count} orders to {filePath}.");
        }
        catch (Exception ex)
        {
            _logger.LogMessage($"Error saving orders to file: {ex.Message}.");
        }
    }

    private bool ValidateOrder(Order order)
    {
        if(order.OrderId == Guid.Empty)
        {
            _logger.LogMessage($"Invalid OrderId: OrderId cannot be an empty {order.OrderId}.");
            return false;
        }

        if (order.Weight < 0.1 || order.Weight > 1000.0)
        {
            _logger.LogMessage($"Invalid Weight: {order.Weight}. Weight must be between 0.1 and 1000.0 kg.");
            return false;
        }

        if (order.IndexRegion < 100000 || order.IndexRegion > 999999)
        {
            _logger.LogMessage($"Invalid IndexRegion: {order.IndexRegion}. It must be a 6-digit number.");
            return false;
        }

        if (order.TimeDelivery == default(DateTime))
        {
            _logger.LogMessage($"{order.TimeDelivery} is missing or has an invalid date.");
            return false;
        }

        return true;
    }
}
