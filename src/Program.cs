using IWantApp.Domain.Users;
using IWantApp.Endpoints.Categories;
using IWantApp.Endpoints.Clients;
using IWantApp.Endpoints.Orders;
using IWantApp.Endpoints.Products;
using IWantApp.Endpoints.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using Serilog.Sinks.MSSqlServer;

var builder = WebApplication.CreateBuilder(args);
/* Gravar Logs no Banco de Dados
builder.WebHost.UseSerilog((context, configuration) =>
{
    configuration
        .WriteTo.Console()
        .WriteTo.MSSqlServer(
            context.Configuration["ConnectionString:IWantDb"],
                sinkOptions: new MSSqlServerSinkOptions()
                {
                    AutoCreateSqlTable = true,
                    TableName = "LogAPI"
                });
});*/

builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration["ConnectionString:IWantDb"]);

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequiredLength = 3;
}).AddEntityFrameworkStores<ApplicationDbContext>();

//builder.Services.AddAuthorization(); //Adicionando Serviço de Autorização
builder.Services.AddAuthorization(options =>//Adicionando Serviço de Autorização onde todas as rotas já vem com [Authorize] por padrão
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
    .RequireAuthenticatedUser()
    .Build();

    options.AddPolicy("EmployeePolicy", p => //Inclusão de uma política específica
     p.RequireAuthenticatedUser().RequireClaim("EmployeeCode")); //Valida apenas se possui o Claim, independente de seu valor
    options.AddPolicy("CpfPolicy", p =>
        p.RequireAuthenticatedUser().RequireClaim("Cpf"));
});
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateActor = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero, //Por padrão o tempo de venc. do token começa a valer apenas 5min após sua geração, aqui esse valor é zerado
        ValidIssuer = builder.Configuration["JwtSecurityTokenSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSecurityTokenSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JwtSecurityTokenSettings:SecretKey"]))
    };
});

builder.Services.AddScoped<QueryAllUsersWithClaimName>(); //Registrar uma classe como serviço
builder.Services.AddScoped<QueryTopSellingProducts>(); //Registrar uma classe como serviço
builder.Services.AddScoped<UserCreator>(); //Registrar uma classe como serviço


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapMethods(CategoryPost.Template, CategoryPost.Methods, CategoryPost.Handle);
app.MapMethods(CategoryGetAll.Template, CategoryGetAll.Methods, CategoryGetAll.Handle);
app.MapMethods(CategoryPut.Template, CategoryPut.Methods, CategoryPut.Handle);
app.MapMethods(EmployeePost.Template, EmployeePost.Methods, EmployeePost.Handle);
app.MapMethods(EmployeeGetAll.Template, EmployeeGetAll.Methods, EmployeeGetAll.Handle);
app.MapMethods(TokenPost.Template, TokenPost.Methods, TokenPost.Handle);
app.MapMethods(ProductPost.Template, ProductPost.Methods, ProductPost.Handle);
app.MapMethods(ProductsGetAll.Template, ProductsGetAll.Methods, ProductsGetAll.Handle);
app.MapMethods(ProductGet.Template, ProductGet.Methods, ProductGet.Handle);
app.MapMethods(ProductGetShowCase.Template, ProductGetShowCase.Methods, ProductGetShowCase.Handle);
app.MapMethods(ClientPost.Template, ClientPost.Methods, ClientPost.Handle);
app.MapMethods(ClientGet.Template, ClientGet.Methods, ClientGet.Handle);
app.MapMethods(OrderPost.Template, OrderPost.Methods, OrderPost.Handle);
app.MapMethods(OrderGet.Template, OrderGet.Methods, OrderGet.Handle);
app.MapMethods(TopSellingProducts.Template, TopSellingProducts.Methods, TopSellingProducts.Handle);

//Filtro de Erros
app.UseExceptionHandler("/error");
app.Map("/error", (HttpContext http) =>
 {
     var error = http.Features?.Get<IExceptionHandlerFeature>()?.Error;
     if(error != null)
     {
         if(error is SqlException)
         {
             return Results.Problem(title: "Database out", statusCode: 500);
         }
         else if(error is BadHttpRequestException)
         {
             return Results.Problem(title: "Error to convert data to other type. See all the information sent", statusCode: 500);
         }
     }
     return Results.Problem(title: "An error ocurred",statusCode: 500);
 });

app.Run();
