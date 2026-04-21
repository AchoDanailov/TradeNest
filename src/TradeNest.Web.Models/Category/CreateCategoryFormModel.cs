using System.ComponentModel.DataAnnotations;

using static TradeNest.GCommon.EntityValidationConstants.Category;
using static TradeNest.GCommon.FormsInvalidInputsNotificationMessages.Category;

namespace TradeNest.Web.Models.Category;

public class CreateCategoryFormModel
{
    [Required(ErrorMessage = NameRequired)]
    [StringLength(NameMaxLengthValue, MinimumLength = NameMinLengthValue, ErrorMessage = CategoryNameLength)]
    public string CategoryName { get; set; } = null!;
}