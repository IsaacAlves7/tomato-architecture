using SharedKernel.Domain;

namespace OrderService.Domain.Aggregates;

/// <summary>
/// Agregado de Pedido — camada "Seed" da Tomato Architecture.
/// Contém todas as regras de negócio do pedido e seus itens.
/// </summary>
public sealed class Order : AggregateRoot<Guid>
{
    private readonly List<OrderItem> _items = new();

    public string CustomerEmail { get; private set; } = string.Empty;
    public string CustomerName { get; private set; } = string.Empty;
    public OrderStatus Status { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string? ShippingAddress { get; private set; }
    public string? Notes { get; private set; }

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    private Order() { }

    private Order(Guid id, string customerEmail, string customerName, string? shippingAddress)
        : base(id)
    {
        CustomerEmail = customerEmail;
        CustomerName = customerName;
        ShippingAddress = shippingAddress;
        Status = OrderStatus.Pending;
        TotalAmount = 0;
    }

    public static Order Create(string customerEmail, string customerName, string? shippingAddress = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customerEmail);
        ArgumentException.ThrowIfNullOrWhiteSpace(customerName);

        var order = new Order(Guid.NewGuid(), customerEmail, customerName, shippingAddress);
        order.AddDomainEvent(new OrderCreatedEvent(order.Id, customerEmail));
        return order;
    }

    public void AddItem(Guid productId, string productName, decimal unitPrice, int quantity)
    {
        if (Status != OrderStatus.Pending)
            throw new OrderDomainException("Só é possível adicionar itens em pedidos pendentes.");

        if (quantity <= 0) throw new OrderDomainException("Quantidade deve ser maior que zero.");
        if (unitPrice < 0) throw new OrderDomainException("Preço unitário inválido.");

        var existing = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existing is not null)
        {
            existing.IncreaseQuantity(quantity);
        }
        else
        {
            _items.Add(OrderItem.Create(Id, productId, productName, unitPrice, quantity));
        }

        RecalculateTotal();
    }

    public void RemoveItem(Guid productId)
    {
        if (Status != OrderStatus.Pending)
            throw new OrderDomainException("Só é possível remover itens de pedidos pendentes.");

        var item = _items.FirstOrDefault(i => i.ProductId == productId)
            ?? throw new OrderDomainException("Item não encontrado no pedido.");

        _items.Remove(item);
        RecalculateTotal();
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new OrderDomainException("Apenas pedidos pendentes podem ser confirmados.");
        if (!_items.Any())
            throw new OrderDomainException("Pedido não pode ser confirmado sem itens.");

        Status = OrderStatus.Confirmed;
        Touch();
        AddDomainEvent(new OrderConfirmedEvent(Id, CustomerEmail, TotalAmount));
    }

    public void Ship()
    {
        if (Status != OrderStatus.Confirmed)
            throw new OrderDomainException("Apenas pedidos confirmados podem ser enviados.");

        Status = OrderStatus.Shipped;
        Touch();
        AddDomainEvent(new OrderShippedEvent(Id, CustomerEmail));
    }

    public void Deliver()
    {
        if (Status != OrderStatus.Shipped)
            throw new OrderDomainException("Apenas pedidos enviados podem ser entregues.");

        Status = OrderStatus.Delivered;
        Touch();
        AddDomainEvent(new OrderDeliveredEvent(Id, CustomerEmail));
    }

    public void Cancel(string reason)
    {
        if (Status is OrderStatus.Delivered or OrderStatus.Cancelled)
            throw new OrderDomainException("Este pedido não pode ser cancelado.");

        Status = OrderStatus.Cancelled;
        Notes = reason;
        Touch();
        AddDomainEvent(new OrderCancelledEvent(Id, CustomerEmail, reason));
    }

    private void RecalculateTotal() =>
        TotalAmount = _items.Sum(i => i.TotalPrice);
}

public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Shipped = 2,
    Delivered = 3,
    Cancelled = 4
}
