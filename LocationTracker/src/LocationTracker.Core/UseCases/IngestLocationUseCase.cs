using LocationTracker.Core.DTOs;
using LocationTracker.Domain.Entities;
using LocationTracker.Domain.Interfaces;

namespace LocationTracker.Core.UseCases;
public class IngestLocationUseCase
{
    private readonly ILocationRepository _locationRepository;
    
    public IngestLocationUseCase(ILocationRepository locationRepository)
    {
        _locationRepository = locationRepository;
    }
    
    public async Task Execute(LocationDto locationDto)
    {
        var locationRecord = new LocationRecord
        {
            DeviceId = locationDto.DeviceId,
            Latitude = locationDto.Latitude,
            Longitude = locationDto.Longitude,
            Timestamp = locationDto.Timestamp,
            Speed = locationDto.Speed,
            Bearing = locationDto.Bearing,
            Accuracy = locationDto.Accuracy,
            Altitude = locationDto.Altitude
        };
        
        await _locationRepository.AddAsync(locationRecord);
    }
}