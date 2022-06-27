using IWantApp.Endpoints.Products;

namespace IWantApp.Infra.Data
{
    public class QueryTopSellingProducts
    {
        public readonly IConfiguration configuration;
        public QueryTopSellingProducts(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task<IEnumerable<TopSellingProductResponse>> Execute(int page, int rows)
        {
            var db = new SqlConnection(configuration["ConnectionString:IWantDb"]);
            var query = @"SELECT o.ProductsId as Id, p.Name, c.Name as CategoryName, p.Description, p.HasStock, p.Price, COUNT(*) as AmountSell
                          FROM OrderProducts o 
                          INNER JOIN Products p ON o.ProductsId = p.Id
                          INNER JOIN Categories c ON p.CategoryId = c.Id
                          WHERE p.Active = 1
                          GROUP by o.ProductsId, p.Name, c.Name, p.Description, p.HasStock, p.Price, p.Active
                          ORDER BY COUNT(*) DESC
                          OFFSET (@page-1)*@rows ROWS FETCH NEXT @rows ROWS ONLY";
            return await db.QueryAsync<TopSellingProductResponse>(query, new { page, rows });
        }
    }
}