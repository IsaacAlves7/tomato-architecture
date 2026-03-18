using AutoMapper;
using MediatR;
using OrderService.Application.DTOs;
using OrderService.Domain.Aggregates;
using OrderService.Domain.Repositories;
using SharedKernel.Common;

namespace OrderService.Application.Queries;

public record GetOrderByIdQuery(Guid Id) : IRequest<Result<OrderDto>>;

public sealed class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Result<OrderDto>>
{
    private readonly IOrderRepository _repository;
    private readonly IMapper _mapper;

    public GetOrderByIdQueryHandler(IOrderRepository repository, IMapper mapper)
    { _repository = repository; _mapper = mapper; }

    public async Task<Result<OrderDto>> Handle(GetOrderByIdQuery request, CancellationToken ct)
    {
        var order = await _repository.GetByIdAsync(request.Id, ct);
        if (order is null) return Result<OrderDto>.Failure("Pedido não encontrado.");
        return Result<OrderDto>.Success(_mapper.Map<OrderDto>(order));
    }
}

public record GetOrdersQuery(int Page = 1, int PageSize = 10, OrderStatus? Status = null)
    : IRequest<PagedResult<OrderDto>>;

public sealed class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, PagedResult<OrderDto>>
{
    private readonly IOrderRepository _repository;
    private readonly IMapper _mapper;

    public GetOrdersQueryHandler(IOrderRepository repository, IMapper mapper)
    { _repository = repository; _mapper = mapper; }

    public async Task<PagedResult<OrderDto>> Handle(GetOrdersQuery request, CancellationToken ct)
    {
        var paged = await _repository.GetAllAsync(request.Page, request.PageSize, request.Status, ct);
        var dtos = _mapper.Map<List<OrderDto>>(paged.Items);
        return new PagedResult<OrderDto>(dtos, paged.TotalCount, paged.Page, paged.PageSize);
    }
}

public record GetOrdersByCustomerQuery(string Email, int Page = 1, int PageSize = 10)
    : IRequest<PagedResult<OrderDto>>;

public sealed class GetOrdersByCustomerQueryHandler : IRequestHandler<GetOrdersByCustomerQuery, PagedResult<OrderDto>>
{
    private readonly IOrderRepository _repository;
    private readonly IMapper _mapper;

    public GetOrdersByCustomerQueryHandler(IOrderRepository repository, IMapper mapper)
    { _repository = repository; _mapper = mapper; }

    public async Task<PagedResult<OrderDto>> Handle(GetOrdersByCustomerQuery request, CancellationToken ct)
    {
        var paged = await _repository.GetByCustomerAsync(request.Email, request.Page, request.PageSize, ct);
        var dtos = _mapper.Map<List<OrderDto>>(paged.Items);
        return new PagedResult<OrderDto>(dtos, paged.TotalCount, paged.Page, paged.PageSize);
    }
}
