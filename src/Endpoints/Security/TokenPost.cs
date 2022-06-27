namespace IWantApp.Endpoints.Security
{
    public class TokenPost
    {
        public static string Template => "/token";
        public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
        public static Delegate Handle => Action;

        [AllowAnonymous]
        public static IResult Action(
            LoginRequest loginRequest,
            IConfiguration configuration, 
            UserManager<IdentityUser> userManager, 
            ILogger<TokenPost> log,
            IWebHostEnvironment environment)
        {
            log.LogInformation("Getting token");

            var user = userManager.FindByEmailAsync(loginRequest.Email).Result;
            if(user == null)
            {
                return Results.BadRequest("Usuário não cadastrado!");
            }
            if (!userManager.CheckPasswordAsync(user, loginRequest.Password).Result)
            {
                return Results.BadRequest("Credenciais inválidas!");
            }

            var claims = userManager.GetClaimsAsync(user).Result;
            var subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Email, loginRequest.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            });
            subject.AddClaims(claims);

            var key = Encoding.ASCII.GetBytes(configuration["JwtSecurityTokenSettings:SecretKey"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = subject,
                SigningCredentials =
                    new SigningCredentials(
                        new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Audience = configuration["JwtSecurityTokenSettings:Audience"],
                Issuer = configuration["JwtSecurityTokenSettings:Issuer"],
                Expires = environment.IsDevelopment() || environment.IsStaging() ? 
                    DateTime.UtcNow.AddYears(1) : DateTime.UtcNow.AddMinutes(Convert.ToDouble(configuration["JwtSecurityTokenSettings:ExpiryTimeInMinutes"])) //Tempo que o token ficará válido, se não informado o valor Default é 60min
            };

            //Gerar Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Results.Ok(new
            {
                token = tokenHandler.WriteToken(token)
            });
        }
    }
}
