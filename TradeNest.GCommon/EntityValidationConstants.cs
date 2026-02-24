namespace TradeNest.GCommon;

/// <summary>
/// This class holds constant values used to validate the correct state of models and DTO's across all layers of the application.
/// </summary>
public static class EntityValidationConstants
{
    public static class Product
    {
        public const byte NameMinLengthValue = 3;
        public const byte NameMaxLengthValue = 255;

        public const byte DescriptionMinLengthValue = 5;
        public const short DescriptionMaxLengthValue = 3000;

        public const byte MinQuantityInStockValue = 0;
        public const short MaxQuantityInStockValue = 10_000;
        
        public const byte MinQuantityToAddToOrder = 1;
        public const short MaxQuantityToAddToOrder = 10_000;

        public const decimal MinSellingPriceValue = 0.01m;

        public const decimal MinCostPriceValue = 0.01m;

        public const byte ExtraImagesUrlsMinLengthValue = 1;
        public const short ExtraImagesUrlsMaxLengthValue 
            = CommonValidationConstants.UrlMaxLengthValue * 10;
        
        public const byte NewImagesUrlsMinLengthValue = 1;
        public const short NewImagesUrlsMaxLengthValue 
            = CommonValidationConstants.UrlMaxLengthValue * 10;

        public const string DefaultValueForCreatedOnColumn = "GETUTCDATE()";

        public const bool DefaultValueForIsEnabledColumn = true;
    }

    public static class User
    {
        public const byte UserNameMinLengthValue = 3;
        public const byte UserNameMaxLengthValue = 40;
        
        public const byte EmailMinLengthValue = 5; 
        public const byte EmailMaxLengthValue = 255;

        public const byte PasswordMinLengthValue = 6;
        public const byte PasswordMaxLengthValue = 100;
    }

    public static class CommonValidationConstants
    {
        public const string PriceColumnDataType = "DECIMAL(10, 2)";
        
        public const byte UrlMinLengthValue = 1;
        public const short UrlMaxLengthValue = 2048;
    }

    public static class Category
    {
        public const byte NameMinLengthValue = 3;
        public const byte NameMaxLengthValue = 100;
    }
}