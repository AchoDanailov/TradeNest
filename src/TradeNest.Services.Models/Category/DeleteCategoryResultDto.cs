namespace TradeNest.Services.Models.Category;

public enum ExpectedFailureReason
{
    NoCategoryToMoveProductsTo = 0,
    RemovingDefaultCategory = 1,
}

/// <summary>
/// Represents the result of the delete category operation.
/// </summary>
public class DeleteCategoryResultDto
{
    public bool IsSuccess { get; }
    public bool WereProductsMoved { get; }
    public ExpectedFailureReason? FailureReason { get; }

    private DeleteCategoryResultDto(
        bool isSuccess = false,
        bool wereProductsMoved = false,
        ExpectedFailureReason? failureReason = null)
    {
        this.IsSuccess = isSuccess;
        this.WereProductsMoved = wereProductsMoved;
        this.FailureReason = failureReason;
    }
    
    public static DeleteCategoryResultDto Success(bool wereProductsMoved = false)
    {
        return new DeleteCategoryResultDto(isSuccess: true, wereProductsMoved: wereProductsMoved);
    }

    public static DeleteCategoryResultDto Failure(ExpectedFailureReason failureReason)
    {
        return new DeleteCategoryResultDto(failureReason: failureReason);
    }
}