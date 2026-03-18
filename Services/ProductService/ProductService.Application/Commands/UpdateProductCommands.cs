using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.DTOs;
using ProductService.Domain.Repositories;
using SharedKernel.Common;

namespace ProductService.Application.Commands;

public record UpdateProductCommand(Guid Id, string Name, string Description, string Category)
    : IRequest<Result<ProductDto>>;

public sealed class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<ProductDto>>
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;

    public UpdateProductCommandHandler(IProductRepository repository, IMapper mapper)
    { _repository = repository; _mapper = mapper; }

    public async Task<Result<ProductDto>> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        var product = await _repository.GetByIdAsync(request.Id, ct);
        if (product is null) return Result<ProductDto>.Failure("Produto não encontrado.");
        product.UpdateDetails(request.Name, request.Description, request.Category);
        await _repository.UpdateAsync(product, ct);
        await _repository.SaveChangesAsync(ct);
        return Result<ProductDto>.Success(_mapper.Map<ProductDto>(product));
    }
}

public record UpdateProductPriceCommand(Guid Id, decimal NewPrice) : IRequest<Result<ProductDto>>;

public sealed class UpdateProductPriceCommandHandler : IRequestHandler<UpdateProductPriceCommand, Result<ProductDto>>
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateProductPriceCommandHandler> _logger;

    public UpdateProductPriceCommandHandler(IProductRepository repository, IMapper mapper, ILogger<UpdateProductPriceCommandHandler> logger)
    { _repository = repository; _mapper = mapper; _logger = logger; }

    public async Task<Result<ProductDto>> Handle(UpdateProductPriceCommand request, CancellationToken ct)
    {
        var product = await _repository.GetByIdAsync(request.Id, ct);
        if (product is null) return Result<ProductDto>.Failure("Produto não encontrado.");
        product.UpdatePrice(request.NewPrice);
        await _repository.UpdateAsync(product, ct);
        await _repository.SaveChangesAsync(ct);
        _logger.LogInformation("Preço do produto {Id} atualizado para {Price}", request.Id, request.NewPrice);
        return Result<ProductDto>.Success(_mapper.Map<ProductDto>(product));
    }
}

public record UpdateStockCommand(Guid Id, int Quantity, StockOperation Operation) : IRequest<Result<ProductDto>>;

public sealed class UpdateStockCommandHandler : IRequestHandler<UpdateStockCommand, Result<ProductDto>>
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;

    public UpdateStockCommandHandler(IProductRepository repository, IMapper mapper)
    { _repository = repository; _mapper = mapper; }

    public async Task<Result<ProductDto>> Handle(UpdateStockCommand request, CancellationToken ct)
    {
        var product = await _repository.GetByIdAsync(request.Id, ct);
        if (product is null) return Result<ProductDto>.Failure("Produto não encontrado.");
        if (request.Operation == StockOperation.Add) product.AddStock(request.Quantity);
        else product.RemoveStock(request.Quantity);
        await _repository.UpdateAsync(product, ct);
        await _repository.SaveChangesAsync(ct);
        return Result<ProductDto>.Success(_mapper.Map<ProductDto>(product));
    }
}

public record DeleteProductCommand(Guid Id) : IRequest<Result>;

public sealed class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Result>
{
    private readonly IProductRepository _repository;

    public DeleteProductCommandHandler(IProductRepository repository) => _repository = repository;

    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken ct)
    {
        if (!await _repository.ExistsAsync(request.Id, ct))
            return Result.Failure("Produto não encontrado.");
        await _repository.DeleteAsync(request.Id, ct);
        await _repository.SaveChangesAsync(ct);
        return Result.Success();
    }
}
