namespace TradeNest.GCommon;

/// <summary>
/// This class holds constant values used to validate the correct state of models and DTO's across all layers of the application.
/// </summary>
public static class EntityValidationConstants
{
    public static class CommonValidationConstants
    {
        public const string PriceColumnDataType = "DECIMAL(10, 2)";
    }
    
    public static class Product
    {
        public const byte NameMinLengthValue = 3;
        public const byte NameMaxLengthValue = 255;

        public const byte DescriptionMinLengthValue = 5;
        public const short DescriptionMaxLengthValue = 3000;

        public const string DefaultValueForCreatedAtColumn = "GETUTCDATE()";

        public const bool DefaultValueForIsEnabledColumn = true;
    }

    public static class Image
    {
        public const short UrlMaxLengthValue = 2048;
    }

    public static class Category
    {
        public const byte NameMinLengthValue = 3;
        public const byte NameMaxLengthValue = 100;
    }
}