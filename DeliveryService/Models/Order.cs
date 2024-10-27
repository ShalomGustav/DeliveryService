using System.ComponentModel.DataAnnotations;

namespace DeliveryService.Models;

public class Order
{
    public Order()
    {
        
    }

    public Order(Guid orderId, double weight, int indexRegion, DateTime timeDelivery)
    {
        OrderId = orderId;
        Weight = weight;
        IndexRegion = indexRegion;
        TimeDelivery = timeDelivery;
    }     
    
    public Guid OrderId { get; set; }

    public double Weight { get; set; }

    public int IndexRegion { get; set; } 

    public DateTime TimeDelivery { get; set; }

    public override string ToString()
    {
        return $"{OrderId}, {Weight},{IndexRegion}, {TimeDelivery.ToString("yyyy-MM-dd HH:mm:ss")}";
    } 
}
