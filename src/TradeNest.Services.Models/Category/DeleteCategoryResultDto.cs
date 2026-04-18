namespace TradeNest.Services.Models.Category;

public enum ExpectedFailureReason
{
    NoCategoryToMoveProductsTo = 0,
}

/// <summary>
/// Represents the result of the delete category operation.
/// </summary>
public class DeleteCategoryResultDto
{
    public bool IsSuccess { get; }
    public ExpectedFailureReason? FailureReason { get; }

    private DeleteCategoryResultDto(
        bool isSuccess = false,
        ExpectedFailureReason? failureReason = null)
    {
        this.IsSuccess = isSuccess;
        this.FailureReason = failureReason;
    }
    
    public static DeleteCategoryResultDto Success()
    {
        return new DeleteCategoryResultDto(isSuccess: true);
    }

    public static DeleteCategoryResultDto Failure(ExpectedFailureReason failureReason)
    {
        return new DeleteCategoryResultDto(failureReason: failureReason);
    }
}