namespace IWantApp.Endpoints.Products
{
    public record TopSellingProductResponse(Guid Id, string Name, string CategoryName, string Description, bool HasStock, decimal Price, int AmountSell);

}
