namespace IWantApp.Endpoints.Products
{
    public class TopSellingProducts
    {
        public static string Template => "/products/topselling";
        public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
        public static Delegate Handle => Action;

        [Authorize(Policy = "EmployeePolicy")]
        public static async Task<IResult> Action(QueryTopSellingProducts query, int page = 1, int rows = 10)
        {
            if (rows > 10)
            {
                return Results.BadRequest("Número máximo de Linhas: 10");
            }
            return Results.Ok(await query.Execute(page, rows));
        }
    }
}
