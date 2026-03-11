using TradeNest.GCommon.Enums;

namespace TradeNest.Web.ViewModels.Cart;

public class ErrorProductViewModel
{
    public IEnumerable<ProductErrorReason> ProductErrorReasons { get; set; }
        = new List<ProductErrorReason>();
    
    public Guid Id { get; set; }

    public string ProductName { get; set; } = null!;
}