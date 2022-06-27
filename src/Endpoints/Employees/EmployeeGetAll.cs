namespace IWantApp.Endpoints.Employees
{
    public class EmployeeGetAll
    {
        public static string Template => "/employees";
        public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
        public static Delegate Handle => Action;

        [Authorize(Policy = "EmployeePolicy")] //Apenas usuários com a policy 'EmployeePolicy' irá conseguir utilizar
        public static async Task<IResult> Action(int? page, int? rows, QueryAllUsersWithClaimName query)
        {
            if(rows > 10)
            {
                return Results.BadRequest("Número máximo de Linhas: 10");
            }
            else
            {
                if (page is null || rows is null)
                {
                    return Results.Ok(await query.Execute());
                }
                else
                {
                    return Results.Ok(await query.Execute((int)page,(int)rows));
                }
            }
        }
    }
}
