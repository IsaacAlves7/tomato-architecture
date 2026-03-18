using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Commands;
using OrderService.Application.DTOs;
using OrderService.Application.Queries;
using OrderService.Domain.Aggregates;

namespace OrderService.API.Controllers;

/// <summary>
/// Controller de Pedidos — camada "Skin" da Tomato Architecture.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public sealed class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator) => _mediator = mediator;

    /// <summary>Lista todos os pedidos com paginação e filtro de status.</summary>
    [HttpGet]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] OrderStatus? status = null,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetOrdersQuery(page, pageSize, status), ct);
        return Ok(result);
    }

    /// <summary>Busca pedido por ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetOrderByIdQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { result.Error });
    }

    /// <summary>Lista pedidos de um cliente por e-mail.</summary>
    [HttpGet("customer/{email}")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetByCustomer(
        string email,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetOrdersByCustomerQuery(email, page, pageSize), ct);
        return Ok(result);
    }

    /// <summary>Cria um novo pedido com itens.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(OrderDto), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create(
        [FromBody] CreateOrderRequest request,
        CancellationToken ct = default)
    {
        var command = new CreateOrderCommand(
            request.CustomerEmail,
            request.CustomerName,
            request.ShippingAddress,
            request.Items);

        var result = await _mediator.Send(command, ct);

        if (result.IsFailure)
            return BadRequest(new { result.Error });

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>Adiciona item a um pedido pendente.</summary>
    [HttpPost("{id:guid}/items")]
    [ProducesResponseType(typeof(OrderDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> AddItem(
        Guid id,
        [FromBody] AddOrderItemRequest request,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new AddOrderItemCommand(id, request.ProductId, request.ProductName, request.UnitPrice, request.Quantity), ct);

        if (result.IsFailure)
            return result.Error!.Contains("não encontrado")
                ? NotFound(new { result.Error })
                : BadRequest(new { result.Error });

        return Ok(result.Value);
    }

    /// <summary>Remove item de um pedido pendente.</summary>
    [HttpDelete("{id:guid}/items/{productId:guid}")]
    [ProducesResponseType(typeof(OrderDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> RemoveItem(
        Guid id,
        Guid productId,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new RemoveOrderItemCommand(id, productId), ct);

        if (result.IsFailure)
            return result.Error!.Contains("não encontrado")
                ? NotFound(new { result.Error })
                : BadRequest(new { result.Error });

        return Ok(result.Value);
    }

    /// <summary>Confirma um pedido pendente.</summary>
    [HttpPost("{id:guid}/confirm")]
    [ProducesResponseType(typeof(OrderDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Confirm(Guid id, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new ConfirmOrderCommand(id), ct);
        return HandleStatusResult(result);
    }

    /// <summary>Marca pedido como enviado.</summary>
    [HttpPost("{id:guid}/ship")]
    [ProducesResponseType(typeof(OrderDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Ship(Guid id, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new ShipOrderCommand(id), ct);
        return HandleStatusResult(result);
    }

    /// <summary>Marca pedido como entregue.</summary>
    [HttpPost("{id:guid}/deliver")]
    [ProducesResponseType(typeof(OrderDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Deliver(Guid id, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new DeliverOrderCommand(id), ct);
        return HandleStatusResult(result);
    }

    /// <summary>Cancela um pedido com motivo.</summary>
    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(typeof(OrderDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Cancel(
        Guid id,
        [FromBody] CancelOrderRequest request,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new CancelOrderCommand(id, request.Reason), ct);
        return HandleStatusResult(result);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────
    private IActionResult HandleStatusResult(SharedKernel.Common.Result<OrderDto> result)
    {
        if (result.IsSuccess) return Ok(result.Value);
        return result.Error!.Contains("não encontrado")
            ? NotFound(new { result.Error })
            : BadRequest(new { result.Error });
    }
}
