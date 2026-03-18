using AutoMapper;
using OrderService.Application.DTOs;
using OrderService.Domain.Aggregates;

namespace OrderService.Application.Mappings;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<Order, OrderDto>()
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));

        CreateMap<OrderItem, OrderItemDto>();
    }
}
