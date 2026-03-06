namespace TradeNest.Services.Models;

public class ProductDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal SellingPrice { get; set; } 

    public string CategoryName { get; set; } = null!;

    public string? FrontImageUrl { get; set; }
}