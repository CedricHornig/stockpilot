namespace StockPilot.Domain.Entities;

public class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Sku { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public int Stock { get; private set; }
    public int LowStockThreshold { get; private set; }
    public Guid WarehouseId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<StockMovement> _movements = new();
    public IReadOnlyCollection<StockMovement> Movements => _movements.AsReadOnly();

    private Product() { }

    public static Product Create(string name, string sku, string category, int lowStockThreshold, Guid warehouseId)
    {
        return new Product
        {
            Id = Guid.NewGuid(),
            Name = name,
            Sku = sku,
            Category = category,
            Stock = 0,
            LowStockThreshold = lowStockThreshold,
            WarehouseId = warehouseId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public bool IsLowStock() => Stock <= LowStockThreshold;
}
