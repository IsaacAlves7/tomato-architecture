using SharedKernel.Domain;

namespace ProductService.Domain.Aggregates;

/// <summary>
/// Agregado de Produto — camada "Seed" da Tomato Architecture.
/// Encapsula toda a lógica e invariantes de negócio do produto.
/// </summary>
public sealed class Product : AggregateRoot<Guid>
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int Stock { get; private set; }
    public string Category { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public string Sku { get; private set; } = string.Empty;

    // EF Core constructor
    private Product() { }

    private Product(
        Guid id,
        string name,
        string description,
        decimal price,
        int stock,
        string category,
        string sku) : base(id)
    {
        Name = name;
        Description = description;
        Price = price;
        Stock = stock;
        Category = category;
        Sku = sku;
        IsActive = true;
    }

    public static Product Create(
        string name,
        string description,
        decimal price,
        int stock,
        string category,
        string sku)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(sku);

        if (price < 0) throw new DomainException("O preço não pode ser negativo.");
        if (stock < 0) throw new DomainException("O estoque não pode ser negativo.");

        var product = new Product(Guid.NewGuid(), name, description, price, stock, category, sku);
        product.AddDomainEvent(new ProductCreatedEvent(product.Id, product.Name, product.Price));
        return product;
    }

    public void UpdateDetails(string name, string description, string category)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
        Description = description;
        Category = category;
        Touch();
        AddDomainEvent(new ProductUpdatedEvent(Id, name));
    }

    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice < 0) throw new DomainException("O preço não pode ser negativo.");
        var oldPrice = Price;
        Price = newPrice;
        Touch();
        AddDomainEvent(new ProductPriceChangedEvent(Id, oldPrice, newPrice));
    }

    public void AddStock(int quantity)
    {
        if (quantity <= 0) throw new DomainException("Quantidade deve ser positiva.");
        Stock += quantity;
        Touch();
        AddDomainEvent(new StockUpdatedEvent(Id, Stock));
    }

    public void RemoveStock(int quantity)
    {
        if (quantity <= 0) throw new DomainException("Quantidade deve ser positiva.");
        if (Stock < quantity) throw new DomainException($"Estoque insuficiente. Disponível: {Stock}");
        Stock -= quantity;
        Touch();
        AddDomainEvent(new StockUpdatedEvent(Id, Stock));
    }

    public void Deactivate()
    {
        IsActive = false;
        Touch();
    }

    public void Activate()
    {
        IsActive = true;
        Touch();
    }
}
