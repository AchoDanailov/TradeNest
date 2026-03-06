namespace TradeNest.Services.Models;

public class OrderDto
{
    public Guid Id { get; set; }
    
    public decimal TotalPrice { get; set; }
    
    public bool IsSubmitted { get; set; }
    
    public DateTime? SubmittedOn { get; set; } 

    public IEnumerable<OrderProductDto> OrderProducts { get; set; }
        = new HashSet<OrderProductDto>();
}