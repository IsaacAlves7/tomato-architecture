using SharedKernel.Domain;

namespace OrderService.Domain.Aggregates;

/// <summary>
/// Item de pedido — entidade filha do agregado Order.
/// </summary>
public sealed class OrderItem : Entity<Guid>
{
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public decimal TotalPrice => UnitPrice * Quantity;

    private OrderItem() { }

    private OrderItem(Guid id, Guid orderId, Guid productId, string productName, decimal unitPrice, int quantity)
        : base(id)
    {
        OrderId = orderId;
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public static OrderItem Create(Guid orderId, Guid productId, string productName, decimal unitPrice, int quantity) =>
        new(Guid.NewGuid(), orderId, productId, productName, unitPrice, quantity);

    public void IncreaseQuantity(int amount)
    {
        if (amount <= 0) throw new OrderDomainException("Quantidade deve ser positiva.");
        Quantity += amount;
    }
}
