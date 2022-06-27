namespace IWantApp.Endpoints.Clients
{
    public class ClientGet
    {
        public static string Template => "/clients";
        public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
        public static Delegate Handle => Action;

        [AllowAnonymous]
        public static async Task<IResult> Action(HttpContext http)
        {
            //Obtendo os dados contidos no TOKEN
            var userId = http.User;
            var result = new
            {
                Id = userId.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value,
                Name = userId.Claims.First(c => c.Type == "Name").Value,
                Cpf = userId.Claims.First(c => c.Type == "Cpf").Value
            };
            return Results.Ok(result);
        }
    }
}
