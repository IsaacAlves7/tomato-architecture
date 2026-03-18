using SharedKernel.Domain;

namespace OrderService.Domain.Aggregates;

// ── Domain Events ─────────────────────────────────────────────────────────────
public record OrderCreatedEvent(Guid OrderId, string CustomerEmail) : DomainEvent;
public record OrderConfirmedEvent(Guid OrderId, string CustomerEmail, decimal TotalAmount) : DomainEvent;
public record OrderShippedEvent(Guid OrderId, string CustomerEmail) : DomainEvent;
public record OrderDeliveredEvent(Guid OrderId, string CustomerEmail) : DomainEvent;
public record OrderCancelledEvent(Guid OrderId, string CustomerEmail, string Reason) : DomainEvent;

// ── Domain Exception ──────────────────────────────────────────────────────────
public class OrderDomainException : Exception
{
    public OrderDomainException(string message) : base(message) { }
}
