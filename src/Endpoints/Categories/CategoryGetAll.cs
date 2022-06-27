namespace IWantApp.Endpoints.Categories
{
    public class CategoryGetAll
    {
        public static string Template => "/categories";
        public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
        public static Delegate Handle => Action;

        public static IResult Action(ApplicationDbContext context)
        {
            var categories = context.Categories.ToList();
            var response = categories.Select(c => new CategoryResponse { Name = c.Name, Active = c.Active, Id = c.Id});

            return Results.Ok(response);
        }
    }
}
