namespace TradeNest.Data.Common;

/// <summary>
/// This class provides with constants used for configuring entity models that EFCore
/// uses to map CLR Models to Relational Model.
/// </summary>
public static class EntityModelsConstants
{
    public static class Product
    {
        public const string DefaultValueForCreatedOnColumn = "GETUTCDATE()";

        public const bool DefaultValueForIsEnabledColumn = true;
    }

    public static class CartProduct
    {
        public const string DefaultValueForAddedOnColumn = "GETUTCDATE()";
    }
        
    public static class Order
    {
        public const string DefaultValueForIsSubmittedOnColumn = "GETUTCDATE()";
    }

    public static class CommonValidationConstants
    {
        public const string PriceColumnDataType = "DECIMAL(10, 2)";
    } 
}