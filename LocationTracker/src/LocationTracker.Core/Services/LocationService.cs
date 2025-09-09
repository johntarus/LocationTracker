using AutoMapper;
using LocationTracker.Core.DTOs;
using LocationTracker.Core.Interfaces.Services;
using LocationTracker.Domain.Entities;
using LocationTracker.Domain.Interfaces;

namespace LocationTracker.Core.Services
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _locationRepository;
        private readonly IMapper _mapper;

        public LocationService(ILocationRepository locationRepository, IMapper mapper)
        {
            _locationRepository = locationRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<DeviceListResponseDto>> GetAllDeviceIdsAsync()
        {
            try
            {
                var deviceIds = await _locationRepository.GetAllDeviceIdsAsync();
                var response = new DeviceListResponseDto { DeviceIds = deviceIds.ToList() };

                return ApiResponse<DeviceListResponseDto>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<DeviceListResponseDto>.CreateError($"Failed to get device IDs: {ex.Message}");
            }
        }

        public async Task<ApiResponse<PaginatedResponse<LocationResponseDto>>> GetLocationsAsync(int page = 1, int pageSize = 10)
        {
            try
            {
                var deviceIds = await _locationRepository.GetAllDeviceIdsAsync();
                var allLocations = new List<LocationRecord>();

                foreach (var deviceId in deviceIds)
                {
                    var deviceLocations = await _locationRepository.GetByDeviceIdAsync(deviceId);
                    allLocations.AddRange(deviceLocations);
                }

                var locationDtos = _mapper.Map<List<LocationResponseDto>>(allLocations)
                    .OrderByDescending(l => l.Timestamp)
                    .ToList();

                var totalCount = locationDtos.Count;

                var items = locationDtos
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var paginatedResponse = new PaginatedResponse<LocationResponseDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize
                };

                return ApiResponse<PaginatedResponse<LocationResponseDto>>.CreateSuccess(paginatedResponse);
            }
            catch (Exception ex)
            {
                return ApiResponse<PaginatedResponse<LocationResponseDto>>.CreateError($"Failed to get locations: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<LocationResponseDto>>> GetLocationsByDeviceAsync(string deviceId)
        {
            try
            {
                var locations = await _locationRepository.GetByDeviceIdAsync(deviceId);

                if (locations == null || !locations.Any())
                {
                    return ApiResponse<List<LocationResponseDto>>.CreateError(
                        $"No locations found for device {deviceId}", new List<LocationResponseDto>());
                }

                var locationDtos = _mapper.Map<List<LocationResponseDto>>(locations)
                    .OrderByDescending(l => l.Timestamp)
                    .ToList();

                return ApiResponse<List<LocationResponseDto>>.CreateSuccess(locationDtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<LocationResponseDto>>.CreateError($"Failed to get locations for {deviceId}: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<LocationResponseDto>>> GetLocationsByDeviceAndTimeRangeAsync(
            string deviceId, DateTime? startTime, DateTime? endTime)
        {
            try
            {
                var locations = await _locationRepository.GetByDeviceIdAsync(deviceId);

                if (locations == null || !locations.Any())
                {
                    return ApiResponse<List<LocationResponseDto>>.CreateError(
                        $"No locations found for device {deviceId}", new List<LocationResponseDto>());
                }

                // Apply filters if provided
                if (startTime.HasValue)
                {
                    locations = locations.Where(l => l.Timestamp >= startTime.Value).ToList();
                }

                if (endTime.HasValue)
                {
                    locations = locations.Where(l => l.Timestamp <= endTime.Value).ToList();
                }

                var locationDtos = _mapper.Map<List<LocationResponseDto>>(locations.OrderBy(l => l.Timestamp));

                return ApiResponse<List<LocationResponseDto>>.CreateSuccess(locationDtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<LocationResponseDto>>.CreateError(
                    $"Failed to get locations for {deviceId}: {ex.Message}");
            }
        }

    }
}
