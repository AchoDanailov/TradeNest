namespace TradeNest.Services.Models.Image;

public class ImageDto
{
    public Guid Id { get; set; }

    public string Url { get; set; } = null!;

    public bool IsMarkedToStay { get; set; } = true;
}