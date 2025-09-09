using LocationTracker.Core.DTOs;

namespace LocationTracker.Core.Interfaces.Services
{
    public interface ILocationService
    {
        Task<ApiResponse<DeviceListResponseDto>> GetAllDeviceIdsAsync();
        Task<ApiResponse<List<LocationResponseDto>>> GetLocationsByDeviceAsync(string deviceId);
        Task<ApiResponse<List<LocationResponseDto>>> GetLocationsByDeviceAndTimeRangeAsync(
            string deviceId, DateTime? startTime, DateTime? endTime);
        Task<ApiResponse<PaginatedResponse<LocationResponseDto>>> GetLocationsAsync(int page = 1, int pageSize = 10);
    }
}