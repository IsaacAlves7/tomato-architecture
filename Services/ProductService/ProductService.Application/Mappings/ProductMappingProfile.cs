using AutoMapper;
using ProductService.Application.DTOs;
using ProductService.Domain.Aggregates;

namespace ProductService.Application.Mappings;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductDto>();
    }
}
