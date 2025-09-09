namespace LocationTracker.Domain.Entities;
public class LocationRecord
{
    public int Id { get; set; }
    public string DeviceId { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime Timestamp { get; set; }
    public double Speed { get; set; } 
    public double Bearing { get; set; }
    public double Accuracy { get; set; } 
    public double Altitude { get; set; }
}