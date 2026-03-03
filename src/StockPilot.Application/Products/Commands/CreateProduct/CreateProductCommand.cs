using MediatR;

namespace StockPilot.Application.Products.Commands.CreateProduct;

public record CreateProductCommand(
    string Name,
    string Sku,
    string Category,
    int LowStockThreshold,
    Guid WarehouseId
) : IRequest<Guid>;
