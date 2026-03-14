namespace TradeNest.Web.ViewModels.Image;

public class ImageViewModel
{
    public Guid Id { get; set; }

    public string Url { get; set; } = null!;

    public bool IsMarkedToStay { get; set; } = true;
}