using LocationTracker.Domain.Entities;
using LocationTracker.Domain.Interfaces;
using LocationTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LocationTracker.Infrastructure.Repositories;
public class LocationRepository : ILocationRepository
{
    private readonly TrackingDbContext _context;
    
    public LocationRepository(TrackingDbContext context)
    {
        _context = context;
    }
    
    public async Task AddAsync(LocationRecord locationRecord)
    {
        await _context.LocationRecords.AddAsync(locationRecord);
        await _context.SaveChangesAsync();
    }
    
    public async Task<IEnumerable<LocationRecord>> GetByDeviceIdAsync(string deviceId)
    {
        return await _context.LocationRecords
            .Where(lr => lr.DeviceId == deviceId)
            .OrderByDescending(lr => lr.Timestamp)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<LocationRecord>> GetByDeviceIdAndTimeRangeAsync(string deviceId, DateTime startTime, DateTime endTime)
    {
        return await _context.LocationRecords
            .Where(lr => lr.DeviceId == deviceId && lr.Timestamp >= startTime && lr.Timestamp <= endTime)
            .OrderBy(lr => lr.Timestamp)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<string>> GetAllDeviceIdsAsync()
    {
        return await _context.LocationRecords
            .Select(lr => lr.DeviceId)
            .Distinct()
            .ToListAsync();
    }
}