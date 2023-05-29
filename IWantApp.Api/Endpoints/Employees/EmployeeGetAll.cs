using IWantApp.Api.Infra.Data;

using Microsoft.AspNetCore.Authorization;

namespace IWantApp.Api.Endpoints.Employees;

public sealed class EmployeeGetAll
{
    public static string Template => "/employees";

    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };

    public static Delegate Handle => Action;

    [Authorize(Policy = "Employee999Policy")]
    public static async Task <IResult> Action(QueryAllUsersWithClaimName query, int page = 1, int rows = 10)
    {
        return Results.Ok(await query.Execute(page, rows).ConfigureAwait(false));
    }

    /*
    public static IResult Action(UserManager<IdentityUser> userManager, int page = 1, int rows = 10)
    {
        var users = userManager.Users
            .AsNoTracking()
            .Skip((page - 1) * rows)
            .Take(rows)
            .ToList()
            .Select<IdentityUser, EmployeeResponse>(u => 
            {
                var claims = userManager.GetClaimsAsync(u).Result;

                var claimName = claims.FirstOrDefault(c => c.Type.Equals("Name"));

                var userName = claimName?.Value != null ? claimName.Value : string.Empty;

                return new EmployeeResponse(userName, u.Email);
            }).OrderBy(e => e.Name);

        return Results.Ok(users);
    }
    */
}