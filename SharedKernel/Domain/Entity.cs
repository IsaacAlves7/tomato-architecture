namespace SharedKernel.Domain;

/// <summary>
/// Entidade base do Kernel Compartilhado.
/// Representa o "Seed" (semente) da Tomato Architecture — o núcleo imutável de identidade.
/// </summary>
public abstract class Entity<TId>
{
    public TId Id { get; protected set; } = default!;
    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }

    protected Entity() { }

    protected Entity(TId id)
    {
        Id = id;
        CreatedAt = DateTime.UtcNow;
    }

    protected void Touch() => UpdatedAt = DateTime.UtcNow;

    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TId> other) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id!.Equals(other.Id);
    }

    public override int GetHashCode() => Id!.GetHashCode();
}
