using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.DTOs;
using ProductService.Domain.Aggregates;
using ProductService.Domain.Repositories;
using SharedKernel.Common;

namespace ProductService.Application.Commands;

// ─── Command ───────────────────────────────────────────────────────────────────
public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    int Stock,
    string Category,
    string Sku
) : IRequest<Result<ProductDto>>;

// ─── Handler ───────────────────────────────────────────────────────────────────
public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<ProductDto>>
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(
        IProductRepository repository,
        IMapper mapper,
        ILogger<CreateProductCommandHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<ProductDto>> Handle(CreateProductCommand request, CancellationToken ct)
    {
        _logger.LogInformation("Criando produto com SKU: {Sku}", request.Sku);

        if (await _repository.SkuExistsAsync(request.Sku, ct))
            return Result<ProductDto>.Failure($"SKU '{request.Sku}' já existe.");

        var product = Product.Create(
            request.Name,
            request.Description,
            request.Price,
            request.Stock,
            request.Category,
            request.Sku
        );

        await _repository.AddAsync(product, ct);
        await _repository.SaveChangesAsync(ct);

        _logger.LogInformation("Produto criado com ID: {ProductId}", product.Id);

        return Result<ProductDto>.Success(_mapper.Map<ProductDto>(product));
    }
}
