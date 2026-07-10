using AutoMapper;
using ProductManagement.Models;
namespace ProductManagement.Options;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ProductViewModel, Product>().ReverseMap();
    }
}
