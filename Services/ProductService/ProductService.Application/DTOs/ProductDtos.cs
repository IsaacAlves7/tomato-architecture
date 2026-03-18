namespace ProductService.Application.DTOs;

public record ProductDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int Stock,
    string Category,
    string Sku,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record CreateProductRequest(
    string Name,
    string Description,
    decimal Price,
    int Stock,
    string Category,
    string Sku
);

public record UpdateProductRequest(
    string Name,
    string Description,
    string Category
);

public record UpdatePriceRequest(decimal NewPrice);

public record UpdateStockRequest(int Quantity, StockOperation Operation);

public enum StockOperation { Add, Remove }
