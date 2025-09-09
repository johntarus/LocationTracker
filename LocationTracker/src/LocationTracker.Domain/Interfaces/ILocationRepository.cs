using LocationTracker.Domain.Entities;

namespace LocationTracker.Domain.Interfaces;
public interface ILocationRepository
{
    Task AddAsync(LocationRecord locationRecord);
    Task<IEnumerable<LocationRecord>> GetByDeviceIdAsync(string deviceId);
    Task<IEnumerable<LocationRecord>> GetByDeviceIdAndTimeRangeAsync(string deviceId, DateTime startTime, DateTime endTime);
    Task<IEnumerable<string>> GetAllDeviceIdsAsync();
}