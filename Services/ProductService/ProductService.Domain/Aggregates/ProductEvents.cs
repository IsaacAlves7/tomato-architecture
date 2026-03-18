using SharedKernel.Domain;

namespace ProductService.Domain.Aggregates;

public record ProductCreatedEvent(Guid ProductId, string Name, decimal Price) : DomainEvent;
public record ProductUpdatedEvent(Guid ProductId, string NewName) : DomainEvent;
public record ProductPriceChangedEvent(Guid ProductId, decimal OldPrice, decimal NewPrice) : DomainEvent;
public record StockUpdatedEvent(Guid ProductId, int NewStock) : DomainEvent;
