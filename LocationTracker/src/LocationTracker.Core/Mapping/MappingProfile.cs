using AutoMapper;
using LocationTracker.Core.DTOs;
using LocationTracker.Domain.Entities;

namespace LocationTracker.Core.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<LocationRecord, LocationResponseDto>()
                .ForMember(dest => dest.FormattedTimestamp,
                    opt => opt.MapFrom(src => src.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")));
        }
    }
}