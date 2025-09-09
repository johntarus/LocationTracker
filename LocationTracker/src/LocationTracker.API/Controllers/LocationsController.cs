using LocationTracker.Core.DTOs;
using LocationTracker.Core.Interfaces.Services;
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
    
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<LocationResponseDto>>>> GetLocations(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var response = await _locationService.GetLocationsAsync(page, pageSize);

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

    [HttpGet("device/{deviceId}/range")]
    public async Task<ActionResult<ApiResponse<List<LocationResponseDto>>>> GetLocationsByDeviceAndTimeRange(
        string deviceId,
        [FromQuery] DateTime? startTime,
        [FromQuery] DateTime? endTime)
    {
        if (string.IsNullOrEmpty(deviceId))
        {
            return BadRequest(ApiResponse<List<LocationResponseDto>>.CreateError("Device ID is required"));
        }

        var response = await _locationService.GetLocationsByDeviceAndTimeRangeAsync(deviceId, startTime, endTime);

        if (!response.Success)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

}
