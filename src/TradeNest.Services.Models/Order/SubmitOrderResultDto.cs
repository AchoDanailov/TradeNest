using System.Collections.ObjectModel;

namespace TradeNest.Services.Models.Order;

/// <summary>
/// Represents the result of the submit order operation.
/// </summary>
public class SubmitOrderResultDto
{
    public bool IsSuccess { get; }
    public IReadOnlyCollection<ErrorProductDto> ErrorProducts { get; }

    private SubmitOrderResultDto(
        bool isSuccess = false,
        IEnumerable<ErrorProductDto>? errorProducts = null)
    {
        this.IsSuccess = isSuccess;
        this.ErrorProducts = errorProducts?.ToList()?.AsReadOnly() ??
                             new ReadOnlyCollection<ErrorProductDto>(new List<ErrorProductDto>());
    }

    public static SubmitOrderResultDto Success()
    {
        return new SubmitOrderResultDto(isSuccess: true);
    }

    public static SubmitOrderResultDto Failure(IEnumerable<ErrorProductDto> errorProducts)
    {
        return new SubmitOrderResultDto(errorProducts: errorProducts);
    }
}