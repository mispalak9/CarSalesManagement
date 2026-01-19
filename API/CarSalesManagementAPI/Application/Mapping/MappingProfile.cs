using AutoMapper;
using CarSalesManagementAPI.Application.DTOs;
using CarSalesManagementAPI.Application.Services;
using CarSalesManagementAPI.Domain.Entities;

namespace CarSalesManagementAPI.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Car Model mappings
        CreateMap<CarModel, CarModelDto>()
            .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand != null ? src.Brand.BrandName : string.Empty))
            .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.CarClass != null ? src.CarClass.ClassName : string.Empty));

        CreateMap<CreateCarModelDto, CarModel>();
        CreateMap<UpdateCarModelDto, CarModel>();

        // Image mappings
        CreateMap<CarModelImage, CarModelImageDto>();

        // Brand and Class mappings
        CreateMap<Brand, BrandDto>();
        CreateMap<CarClass, CarClassDto>();
    }
}
