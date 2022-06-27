namespace IWantApp.Infra.Data
{
    public class QueryAllUsersWithClaimName
    {
        public readonly IConfiguration configuration;
        public QueryAllUsersWithClaimName(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task<IEnumerable<EmployeeResponse>> Execute(int page, int rows)
        {
            var db = new SqlConnection(configuration["ConnectionString:IWantDb"]);
            var query =
                @"SELECT u.Email, c.ClaimValue as Name
                FROM AspNetUsers u
                INNER JOIN AspnetUserClaims c
                ON u.Id = c.UserId and claimtype = 'Name'
                ORDER BY Name
                OFFSET (@page-1)*@rows ROWS FETCH NEXT @rows ROWS ONLY";
            return await db.QueryAsync<EmployeeResponse>( //DAPPER CONVERTE UM RESULTADO SQL EM UMA CLASSE JÁ EXISTENTE
            query,
            new { page, rows } //Parametros da consulta
            );
        }
        public async Task<IEnumerable<EmployeeResponse>> Execute()
        {
            var db = new SqlConnection(configuration["ConnectionString:IWantDb"]);
            var query =
                @"SELECT u.Email, c.ClaimValue as Name
                FROM AspNetUsers u
                INNER JOIN AspnetUserClaims c
                ON u.Id = c.UserId and claimtype = 'Name'
                ORDER BY Name";
            return await db.QueryAsync<EmployeeResponse>( //DAPPER CONVERTE UM RESULTADO SQL EM UMA CLASSE JÁ EXISTENTE
            query);
        }
    }
}
