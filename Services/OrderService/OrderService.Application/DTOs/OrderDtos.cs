using OrderService.Domain.Aggregates;

namespace OrderService.Application.DTOs;

public record OrderDto(
    Guid Id,
    string CustomerEmail,
    string CustomerName,
    string Status,
    decimal TotalAmount,
    string? ShippingAddress,
    string? Notes,
    IReadOnlyCollection<OrderItemDto> Items,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record OrderItemDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal TotalPrice
);

public record CreateOrderRequest(
    string CustomerEmail,
    string CustomerName,
    string? ShippingAddress,
    List<OrderItemRequest> Items
);

public record OrderItemRequest(
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity
);

public record AddOrderItemRequest(
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity
);

public record CancelOrderRequest(string Reason);
