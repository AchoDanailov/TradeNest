using System.Security.Cryptography;

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

    public static class User
    {
        public const byte UserNameMinValidLengthValue = 4;
        /// <summary>
        /// Value is taken from Identity System's AspNetUser entity model's UserName column max length value.
        /// </summary>
        public const short UserNameMaxValidLengthValue = 256;
    }
    
    public static class Product
    {
        public const byte NameMinLengthValue = 3;
        public const byte NameMaxLengthValue = 255;

        public const byte DescriptionMinLengthValue = 5;
        public const short DescriptionMaxLengthValue = 3000;

        public const byte MinQuantityInStockValue = 0;
        public const short MaxQuantityInStockValue = 10_000;

        public const decimal MinSellingPriceValue = 0.01m;

        public const string DefaultValueForCreatedOnColumn = "GETUTCDATE()";

        public const bool DefaultValueForIsEnabledColumn = true;
    }

    public static class Image
    {
        public const byte UrlMinLengthValue = 1;
        public const short UrlMaxLengthValue = 2048;
    }

    public static class Category
    {
        public const byte NameMinLengthValue = 3;
        public const byte NameMaxLengthValue = 100;
    }
}