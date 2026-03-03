namespace StockPilot.Domain.Entities;

public class Warehouse
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string TenantId { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    private readonly List<Product> _products = new();
    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

    private Warehouse() { }

    public static Warehouse Create(string name, string tenantId)
    {
        return new Warehouse
        {
            Id = Guid.NewGuid(),
            Name = name,
            TenantId = tenantId,
            CreatedAt = DateTime.UtcNow
        };
    }
}
