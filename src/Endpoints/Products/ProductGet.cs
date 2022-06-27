namespace IWantApp.Endpoints.Products
{
    public class ProductGet
    {
        public static string Template => "/products/{id:Guid}";
        public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
        public static Delegate Handle => Action;

        [Authorize(Policy = "EmployeePolicy")]
        public static IResult Action([FromRoute] Guid id, ApplicationDbContext context)
        {
            var products = context.Products.Include(p => p.Category).OrderBy(p => p.Name).ToList();
            var result = products.FirstOrDefault(p => p.Id == id );
            if(result == null)
            {
                return Results.BadRequest("Produto não cadastrado!");
            }
            return Results.Ok(new ProductResponse(result.Id, result.Name, result.Category.Name, result.Description, result.HasStock,result.Price, result.Active));
        }
    }
}
