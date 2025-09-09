using LocationTracker.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LocationTracker.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class LocationsController : ControllerBase
{
    private readonly ILocationRepository _locationRepository;
    
    public LocationsController(ILocationRepository locationRepository)
    {
        _locationRepository = locationRepository;
    }
    
    [HttpGet("devices")]
    public async Task<IActionResult> GetDevices()
    {
        var deviceIds = await _locationRepository.GetAllDeviceIdsAsync();
        return Ok(deviceIds);
    }
    
    [HttpGet("device/{deviceId}")]
    public async Task<IActionResult> GetLocationsByDevice(string deviceId)
    {
        var locations = await _locationRepository.GetByDeviceIdAsync(deviceId);
        return Ok(locations);
    }
    
    [HttpGet("device/{deviceId}/range")]
    public async Task<IActionResult> GetLocationsByDeviceAndTimeRange(
        string deviceId, 
        [FromQuery] DateTime startTime, 
        [FromQuery] DateTime endTime)
    {
        var locations = await _locationRepository.GetByDeviceIdAndTimeRangeAsync(deviceId, startTime, endTime);
        return Ok(locations);
    }
}