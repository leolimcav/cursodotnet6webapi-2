using Dapper;

using IWantApp.Api.Endpoints.Employees;

using Microsoft.Data.SqlClient;

namespace IWantApp.Api.Infra.Data;

public sealed class QueryAllUsersWithClaimName
{
   private readonly SqlConnection _sqlConnection;

   public QueryAllUsersWithClaimName(SqlConnection sqlConnection)
   {
       this._sqlConnection = sqlConnection;
   }

   public async Task<IEnumerable<EmployeeResponse>> Execute(int page, int rows)
   {
        return await this._sqlConnection.QueryAsync<EmployeeResponse>(@"
                SELECT ClaimValue as Name, Email
                FROM AspNetUsers u
                INNER JOIN AspNetUserClaims c
                ON u.id = c.UserId
                AND c.ClaimType = 'Name'
                ORDER BY Name
                OFFSET (@page - 1) * @rows ROWS 
                FETCH NEXT @rows ROWS ONLY",
                new { page, rows }).ConfigureAwait(false);
   }
}