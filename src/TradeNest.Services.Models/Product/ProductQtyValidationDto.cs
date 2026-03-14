namespace TradeNest.Services.Models.Product;

public class ProductQtyValidationDto
{
    public Guid Id { get; set; }
    
    public int QuantityRequested { get; set; }
}