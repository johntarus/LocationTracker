namespace LocationTracker.Core.DTOs;

public class LocationResponseDto
{
    public string DeviceId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime Timestamp { get; set; }
    public double? Speed { get; set; }
    public double? Bearing { get; set; }
    public double? Accuracy { get; set; }
    public double? Altitude { get; set; }
    public string FormattedTimestamp => Timestamp.ToString("yyyy-MM-dd HH:mm:ss");
}

public class DeviceListResponseDto
{
    public List<string> DeviceIds { get; set; }
    public int Count => DeviceIds?.Count ?? 0;
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
    public DateTime ResponseTime { get; set; } = DateTime.UtcNow;

    public static ApiResponse<T> CreateSuccess(T data, string message = "Success")
    {
        return new ApiResponse<T> 
        { 
            Success = true, 
            Message = message, 
            Data = data 
        };
    }
    
    public static ApiResponse<T> CreateError(string message, T data = default)
    {
        return new ApiResponse<T> 
        { 
            Success = false, 
            Message = message, 
            Data = data 
        };
    }
}

public class PaginatedResponse<T>
{
    public List<T> Items { get; set; }
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}