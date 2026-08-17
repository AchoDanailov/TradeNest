namespace TradeNest.GCommon;

/// <summary>
/// Provides constant string messages used for notifying the user of wrong inputs when filling an application form.
/// </summary>
public static class FormsInvalidInputsNotificationMessages
{
    public static class Product
    {
        public const string ProductIdRequired = "Something went wrong; we couldn't identify the product you're trying to edit.";
        public const string ProductNameRequired = "Please provide a title for the product.";
        public const string ProductNameLength = "The product title should be between {2} and {1} characters long.";

        public const string DescriptionRequired = "A description is required so buyers know what they are getting.";
        public const string DescriptionLength = "The description should be between {2} and {1} characters long.";

        public const string QuantityInStockRange = "The quantity must be a number between {1} and {2}.";

        public const string SellingPriceRequired = "Please set a selling price for the product.";
        public const string SellingPriceRange = "The selling price must be a larger amount than €{1}.";

        public const string CostPriceRange = "The cost price must be a larger amount than €{1}.";

        public const string CategoryRequired = "Please select a category that best fits your product.";

        public const string FrontImageUrlLength = "The main image link is too long. Please use a shorter URL.";
        public const string ExtraImagesUrlsLength = "The additional image links are too long in total. Please remove some or use shorter URLs.";
        public const string NewImagesUrlsLength = "The new image links are too long in total. Please remove some or use shorter URLs.";
    }

    public static class Category
    {
        public const string NameRequired = "Please provide a name for the category.";
        public const string CategoryNameLength = "The Category name should be between {2} and {1} characters long.";
    }
}
