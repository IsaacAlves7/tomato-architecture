using AutoMapper;
using MediatR;
using ProductService.Application.DTOs;
using ProductService.Domain.Repositories;
using SharedKernel.Common;

namespace ProductService.Application.Queries;

// ─── Get By ID ─────────────────────────────────────────────────────────────────
public record GetProductByIdQuery(Guid Id) : IRequest<Result<ProductDto>>;

public sealed class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(IProductRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken ct)
    {
        var product = await _repository.GetByIdAsync(request.Id, ct);
        if (product is null) return Result<ProductDto>.Failure("Produto não encontrado.");
        return Result<ProductDto>.Success(_mapper.Map<ProductDto>(product));
    }
}

// ─── Get All (paginado) ────────────────────────────────────────────────────────
public record GetProductsQuery(int Page = 1, int PageSize = 10, string? Category = null)
    : IRequest<PagedResult<ProductDto>>;

public sealed class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, PagedResult<ProductDto>>
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;

    public GetProductsQueryHandler(IProductRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PagedResult<ProductDto>> Handle(GetProductsQuery request, CancellationToken ct)
    {
        var paged = await _repository.GetAllAsync(request.Page, request.PageSize, request.Category, ct);
        var dtos = _mapper.Map<List<ProductDto>>(paged.Items);
        return new PagedResult<ProductDto>(dtos, paged.TotalCount, paged.Page, paged.PageSize);
    }
}

// ─── Get By SKU ────────────────────────────────────────────────────────────────
public record GetProductBySkuQuery(string Sku) : IRequest<Result<ProductDto>>;

public sealed class GetProductBySkuQueryHandler : IRequestHandler<GetProductBySkuQuery, Result<ProductDto>>
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;

    public GetProductBySkuQueryHandler(IProductRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<ProductDto>> Handle(GetProductBySkuQuery request, CancellationToken ct)
    {
        var product = await _repository.GetBySkuAsync(request.Sku, ct);
        if (product is null) return Result<ProductDto>.Failure("Produto não encontrado.");
        return Result<ProductDto>.Success(_mapper.Map<ProductDto>(product));
    }
}
