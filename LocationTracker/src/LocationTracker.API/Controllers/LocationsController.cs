using LocationTracker.Core.DTOs;
using LocationTracker.Core.Services;
using LocationTracker.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LocationTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LocationsController : ControllerBase
{
    private readonly ILocationService _locationService;
    
    public LocationsController(ILocationService locationService)
    {
        _locationService = locationService;
    }
    
    // GET: api/locations
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<LocationResponseDto>>>> GetAllLocations()
    {
        var response = await _locationService.GetAllLocationsAsync();
        
        if (!response.Success)
        {
            return StatusCode(500, response);
        }
        
        return Ok(response);
    }
    
    // GET: api/locations/paginated?page=1&pageSize=20
    [HttpGet("paginated")]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<LocationResponseDto>>>> GetPaginatedLocations(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 20)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var response = await _locationService.GetPaginatedLocationsAsync(page, pageSize);
        
        if (!response.Success)
        {
            return StatusCode(500, response);
        }
        
        return Ok(response);
    }
    
    // GET: api/locations/devices
    [HttpGet("devices")]
    public async Task<ActionResult<ApiResponse<DeviceListResponseDto>>> GetDevices()
    {
        var response = await _locationService.GetAllDeviceIdsAsync();
        
        if (!response.Success)
        {
            return StatusCode(500, response);
        }
        
        return Ok(response);
    }
    
    // GET: api/locations/device/{deviceId}
    [HttpGet("device/{deviceId}")]
    public async Task<ActionResult<ApiResponse<List<LocationResponseDto>>>> GetLocationsByDevice(string deviceId)
    {
        if (string.IsNullOrEmpty(deviceId))
        {
            return BadRequest(ApiResponse<List<LocationResponseDto>>.CreateError("Device ID is required"));
        }

        var response = await _locationService.GetLocationsByDeviceAsync(deviceId);
        
        if (!response.Success)
        {
            return NotFound(response);
        }
        
        return Ok(response);
    }
    
    // GET: api/locations/device/{deviceId}/range
    [HttpGet("device/{deviceId}/range")]
    public async Task<ActionResult<ApiResponse<List<LocationResponseDto>>>> GetLocationsByDeviceAndTimeRange(
        string deviceId, 
        [FromQuery] DateTime startTime, 
        [FromQuery] DateTime endTime)
    {
        if (string.IsNullOrEmpty(deviceId))
        {
            return BadRequest(ApiResponse<List<LocationResponseDto>>.CreateError("Device ID is required"));
        }

        if (startTime >= endTime)
        {
            return BadRequest(ApiResponse<List<LocationResponseDto>>.CreateError("Start time must be before end time"));
        }

        var response = await _locationService.GetLocationsByDeviceAndTimeRangeAsync(deviceId, startTime, endTime);
        
        if (!response.Success)
        {
            return NotFound(response);
        }
        
        return Ok(response);
    }
    
    // GET: api/locations/recent
    [HttpGet("recent")]
    public async Task<ActionResult<ApiResponse<List<LocationResponseDto>>>> GetRecentLocations([FromQuery] int count = 10)
    {
        if (count < 1) count = 10;
        if (count > 100) count = 100;

        var response = await _locationService.GetRecentLocationsAsync(count);
        
        if (!response.Success)
        {
            return StatusCode(500, response);
        }
        
        return Ok(response);
    }
    
    // POST: api/locations
    [HttpPost]
    public async Task<ActionResult<ApiResponse<LocationResponseDto>>> AddLocation([FromBody] LocationRecord location)
    {
        if (location == null)
        {
            return BadRequest(ApiResponse<LocationResponseDto>.CreateError("Location data is required"));
        }

        if (string.IsNullOrEmpty(location.DeviceId))
        {
            return BadRequest(ApiResponse<LocationResponseDto>.CreateError("Device ID is required"));
        }

        var response = await _locationService.AddLocationAsync(location);
        
        if (!response.Success)
        {
            return StatusCode(500, response);
        }
        
        return CreatedAtAction(nameof(GetLocationsByDevice), new { deviceId = location.DeviceId }, response);
    }
}