using AutoMapper;

namespace WebApp.Helpers;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<App.DAL.DTO.Storage, App.DTO.v1_0.Storage>().ReverseMap();
        CreateMap<App.DAL.DTO.Item, App.DTO.v1_0.Item>().ReverseMap();
    }
}