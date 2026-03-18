using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OrderService.Application.DTOs;
using OrderService.Domain.Aggregates;
using OrderService.Domain.Repositories;
using SharedKernel.Common;

namespace OrderService.Application.Commands;

// ── Create Order ──────────────────────────────────────────────────────────────
public record CreateOrderCommand(
    string CustomerEmail,
    string CustomerName,
    string? ShippingAddress,
    List<OrderItemRequest> Items
) : IRequest<Result<OrderDto>>;

public sealed class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<OrderDto>>
{
    private readonly IOrderRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateOrderCommandHandler> _logger;

    public CreateOrderCommandHandler(IOrderRepository repository, IMapper mapper, ILogger<CreateOrderCommandHandler> logger)
    { _repository = repository; _mapper = mapper; _logger = logger; }

    public async Task<Result<OrderDto>> Handle(CreateOrderCommand req, CancellationToken ct)
    {
        if (!req.Items.Any())
            return Result<OrderDto>.Failure("O pedido precisa ter pelo menos um item.");

        var order = Order.Create(req.CustomerEmail, req.CustomerName, req.ShippingAddress);

        foreach (var item in req.Items)
            order.AddItem(item.ProductId, item.ProductName, item.UnitPrice, item.Quantity);

        await _repository.AddAsync(order, ct);
        await _repository.SaveChangesAsync(ct);

        _logger.LogInformation("Pedido {OrderId} criado para {Email}", order.Id, req.CustomerEmail);
        return Result<OrderDto>.Success(_mapper.Map<OrderDto>(order));
    }
}

// ── Add Item ──────────────────────────────────────────────────────────────────
public record AddOrderItemCommand(Guid OrderId, Guid ProductId, string ProductName, decimal UnitPrice, int Quantity)
    : IRequest<Result<OrderDto>>;

public sealed class AddOrderItemCommandHandler : IRequestHandler<AddOrderItemCommand, Result<OrderDto>>
{
    private readonly IOrderRepository _repository;
    private readonly IMapper _mapper;

    public AddOrderItemCommandHandler(IOrderRepository repository, IMapper mapper)
    { _repository = repository; _mapper = mapper; }

    public async Task<Result<OrderDto>> Handle(AddOrderItemCommand req, CancellationToken ct)
    {
        var order = await _repository.GetByIdAsync(req.OrderId, ct);
        if (order is null) return Result<OrderDto>.Failure("Pedido não encontrado.");

        try { order.AddItem(req.ProductId, req.ProductName, req.UnitPrice, req.Quantity); }
        catch (OrderDomainException ex) { return Result<OrderDto>.Failure(ex.Message); }

        await _repository.UpdateAsync(order, ct);
        await _repository.SaveChangesAsync(ct);
        return Result<OrderDto>.Success(_mapper.Map<OrderDto>(order));
    }
}

// ── Remove Item ───────────────────────────────────────────────────────────────
public record RemoveOrderItemCommand(Guid OrderId, Guid ProductId) : IRequest<Result<OrderDto>>;

public sealed class RemoveOrderItemCommandHandler : IRequestHandler<RemoveOrderItemCommand, Result<OrderDto>>
{
    private readonly IOrderRepository _repository;
    private readonly IMapper _mapper;

    public RemoveOrderItemCommandHandler(IOrderRepository repository, IMapper mapper)
    { _repository = repository; _mapper = mapper; }

    public async Task<Result<OrderDto>> Handle(RemoveOrderItemCommand req, CancellationToken ct)
    {
        var order = await _repository.GetByIdAsync(req.OrderId, ct);
        if (order is null) return Result<OrderDto>.Failure("Pedido não encontrado.");

        try { order.RemoveItem(req.ProductId); }
        catch (OrderDomainException ex) { return Result<OrderDto>.Failure(ex.Message); }

        await _repository.UpdateAsync(order, ct);
        await _repository.SaveChangesAsync(ct);
        return Result<OrderDto>.Success(_mapper.Map<OrderDto>(order));
    }
}

// ── Status Transitions ────────────────────────────────────────────────────────
public record ConfirmOrderCommand(Guid OrderId) : IRequest<Result<OrderDto>>;
public record ShipOrderCommand(Guid OrderId) : IRequest<Result<OrderDto>>;
public record DeliverOrderCommand(Guid OrderId) : IRequest<Result<OrderDto>>;
public record CancelOrderCommand(Guid OrderId, string Reason) : IRequest<Result<OrderDto>>;

public sealed class ConfirmOrderCommandHandler : IRequestHandler<ConfirmOrderCommand, Result<OrderDto>>
{
    private readonly IOrderRepository _repo; private readonly IMapper _mapper;
    public ConfirmOrderCommandHandler(IOrderRepository repo, IMapper mapper) { _repo = repo; _mapper = mapper; }
    public async Task<Result<OrderDto>> Handle(ConfirmOrderCommand req, CancellationToken ct)
    {
        var order = await _repo.GetByIdAsync(req.OrderId, ct);
        if (order is null) return Result<OrderDto>.Failure("Pedido não encontrado.");
        try { order.Confirm(); } catch (OrderDomainException ex) { return Result<OrderDto>.Failure(ex.Message); }
        await _repo.UpdateAsync(order, ct); await _repo.SaveChangesAsync(ct);
        return Result<OrderDto>.Success(_mapper.Map<OrderDto>(order));
    }
}

public sealed class ShipOrderCommandHandler : IRequestHandler<ShipOrderCommand, Result<OrderDto>>
{
    private readonly IOrderRepository _repo; private readonly IMapper _mapper;
    public ShipOrderCommandHandler(IOrderRepository repo, IMapper mapper) { _repo = repo; _mapper = mapper; }
    public async Task<Result<OrderDto>> Handle(ShipOrderCommand req, CancellationToken ct)
    {
        var order = await _repo.GetByIdAsync(req.OrderId, ct);
        if (order is null) return Result<OrderDto>.Failure("Pedido não encontrado.");
        try { order.Ship(); } catch (OrderDomainException ex) { return Result<OrderDto>.Failure(ex.Message); }
        await _repo.UpdateAsync(order, ct); await _repo.SaveChangesAsync(ct);
        return Result<OrderDto>.Success(_mapper.Map<OrderDto>(order));
    }
}

public sealed class DeliverOrderCommandHandler : IRequestHandler<DeliverOrderCommand, Result<OrderDto>>
{
    private readonly IOrderRepository _repo; private readonly IMapper _mapper;
    public DeliverOrderCommandHandler(IOrderRepository repo, IMapper mapper) { _repo = repo; _mapper = mapper; }
    public async Task<Result<OrderDto>> Handle(DeliverOrderCommand req, CancellationToken ct)
    {
        var order = await _repo.GetByIdAsync(req.OrderId, ct);
        if (order is null) return Result<OrderDto>.Failure("Pedido não encontrado.");
        try { order.Deliver(); } catch (OrderDomainException ex) { return Result<OrderDto>.Failure(ex.Message); }
        await _repo.UpdateAsync(order, ct); await _repo.SaveChangesAsync(ct);
        return Result<OrderDto>.Success(_mapper.Map<OrderDto>(order));
    }
}

public sealed class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Result<OrderDto>>
{
    private readonly IOrderRepository _repo; private readonly IMapper _mapper;
    public CancelOrderCommandHandler(IOrderRepository repo, IMapper mapper) { _repo = repo; _mapper = mapper; }
    public async Task<Result<OrderDto>> Handle(CancelOrderCommand req, CancellationToken ct)
    {
        var order = await _repo.GetByIdAsync(req.OrderId, ct);
        if (order is null) return Result<OrderDto>.Failure("Pedido não encontrado.");
        try { order.Cancel(req.Reason); } catch (OrderDomainException ex) { return Result<OrderDto>.Failure(ex.Message); }
        await _repo.UpdateAsync(order, ct); await _repo.SaveChangesAsync(ct);
        return Result<OrderDto>.Success(_mapper.Map<OrderDto>(order));
    }
}
