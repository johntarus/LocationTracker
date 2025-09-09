using AutoMapper;
using LocationTracker.Core.DTOs;
using LocationTracker.Core.Services;
using LocationTracker.Domain.Entities;
using LocationTracker.Domain.Interfaces;
using Moq;
using Xunit;

namespace LocationTracker.Tests.Services
{
    public class LocationServiceTests
    {
        private readonly Mock<ILocationRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly LocationService _locationService;

        public LocationServiceTests()
        {
            _mockRepository = new Mock<ILocationRepository>();
            _mockMapper = new Mock<IMapper>();
            _locationService = new LocationService(_mockRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetAllDeviceIdsAsync_ReturnsDeviceIds()
        {
            var deviceIds = new List<string> { "device-001", "device-002" };
            _mockRepository.Setup(r => r.GetAllDeviceIdsAsync())
                .ReturnsAsync(deviceIds);

            var result = await _locationService.GetAllDeviceIdsAsync();

            Xunit.Assert.True(result.Success);
            Xunit.Assert.Equal(2, result.Data.DeviceIds.Count);
            Xunit.Assert.Equal("device-001", result.Data.DeviceIds[0]);
        }

        [Fact]
        public async Task GetAllDeviceIdsAsync_ReturnsError_OnException()
        {
            _mockRepository.Setup(r => r.GetAllDeviceIdsAsync())
                .ThrowsAsync(new Exception("DB error"));

            var result = await _locationService.GetAllDeviceIdsAsync();

            Xunit.Assert.False(result.Success);
            Xunit.Assert.Contains("DB error", result.Message);
        }

        [Fact]
        public async Task GetLocationsByDeviceAsync_ReturnsMappedLocations()
        {
            var deviceId = "device-001";
            var records = new List<LocationRecord>
            {
                new LocationRecord { DeviceId = deviceId, Timestamp = DateTime.UtcNow }
            };

            var mappedDtos = new List<LocationResponseDto>
            {
                new LocationResponseDto { DeviceId = deviceId, Timestamp = DateTime.UtcNow }
            };

            _mockRepository.Setup(r => r.GetByDeviceIdAsync(deviceId))
                .ReturnsAsync(records);
            _mockMapper.Setup(m => m.Map<List<LocationResponseDto>>(records))
                .Returns(mappedDtos);

            var result = await _locationService.GetLocationsByDeviceAsync(deviceId);

            Xunit.Assert.True(result.Success);
            Xunit.Assert.Single(result.Data);
            Xunit.Assert.Equal(deviceId, result.Data[0].DeviceId);
        }
    }
}