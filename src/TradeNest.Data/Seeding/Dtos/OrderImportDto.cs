using System.ComponentModel.DataAnnotations;

namespace TradeNest.Data.Seeding.Dtos;

public class OrderImportDto
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public DateTime SubmittedOn { get; set; }

    [Required]
    public decimal TotalPrice { get; set; }

    [Required]
    public Guid UserId { get; set; }

    public ICollection<OrderProductImportDto> OrderProducts { get; set; }
        = new List<OrderProductImportDto>();
}