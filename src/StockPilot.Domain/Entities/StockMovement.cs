using StockPilot.Domain.Enums;

namespace StockPilot.Domain.Entities;

public class StockMovement
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public MovementType Type { get; private set; }
    public int Quantity { get; private set; }
    public string Note { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    private StockMovement() { }

    public static StockMovement Create(Guid productId, MovementType type, int quantity, string note = "")
    {
        return new StockMovement
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            Type = type,
            Quantity = quantity,
            Note = note,
            CreatedAt = DateTime.UtcNow
        };
    }
}
