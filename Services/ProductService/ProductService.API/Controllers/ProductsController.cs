using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Commands;
using ProductService.Application.DTOs;
using ProductService.Application.Queries;

namespace ProductService.API.Controllers;

/// <summary>
/// Controller de Produtos — camada "Skin" da Tomato Architecture.
/// Delega tudo ao Mediator, sem lógica de negócio aqui.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public sealed class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator) => _mediator = mediator;

    /// <summary>Lista produtos com paginação opcional por categoria.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), 200)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? category = null,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetProductsQuery(page, pageSize, category), ct);
        return Ok(result);
    }

    /// <summary>Busca produto por ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetProductByIdQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { result.Error });
    }

    /// <summary>Busca produto por SKU.</summary>
    [HttpGet("sku/{sku}")]
    [ProducesResponseType(typeof(ProductDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetBySku(string sku, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetProductBySkuQuery(sku), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { result.Error });
    }

    /// <summary>Cria um novo produto.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> Create(
        [FromBody] CreateProductRequest request,
        CancellationToken ct = default)
    {
        var command = new CreateProductCommand(
            request.Name, request.Description,
            request.Price, request.Stock,
            request.Category, request.Sku);

        var result = await _mediator.Send(command, ct);

        if (result.IsFailure)
            return result.Error!.Contains("já existe") ? Conflict(new { result.Error }) : BadRequest(new { result.Error });

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>Atualiza dados do produto.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProductDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateProductRequest request,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new UpdateProductCommand(id, request.Name, request.Description, request.Category), ct);

        return result.IsSuccess ? Ok(result.Value) : NotFound(new { result.Error });
    }

    /// <summary>Atualiza preço do produto.</summary>
    [HttpPatch("{id:guid}/price")]
    [ProducesResponseType(typeof(ProductDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdatePrice(
        Guid id,
        [FromBody] UpdatePriceRequest request,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new UpdateProductPriceCommand(id, request.NewPrice), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { result.Error });
    }

    /// <summary>Atualiza estoque do produto (adicionar ou remover).</summary>
    [HttpPatch("{id:guid}/stock")]
    [ProducesResponseType(typeof(ProductDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateStock(
        Guid id,
        [FromBody] UpdateStockRequest request,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new UpdateStockCommand(id, request.Quantity, request.Operation), ct);

        if (result.IsFailure)
            return result.Error!.Contains("não encontrado") ? NotFound(new { result.Error }) : BadRequest(new { result.Error });

        return Ok(result.Value);
    }

    /// <summary>Remove um produto.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new DeleteProductCommand(id), ct);
        return result.IsSuccess ? NoContent() : NotFound(new { result.Error });
    }
}
