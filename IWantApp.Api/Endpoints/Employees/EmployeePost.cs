using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace IWantApp.Api.Endpoints.Employees;

public sealed class EmployeePost
{
    public static string Template => "/employees";

    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };

    public static Delegate Handle => Action;

    [Authorize(Policy = "EmployeePolicy")]
    public static async Task<IResult> Action(EmployeeRequest employeeRequest, UserManager<IdentityUser> userManager, HttpContext http)
    {
       
        var userId = http.User.Claims.First(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;
        var newUser =  new IdentityUser 
        {
            UserName = employeeRequest.Email,
            Email = employeeRequest.Email
        };
        var result = await userManager.CreateAsync(newUser, employeeRequest.Password).ConfigureAwait(false);

        if(!result.Succeeded) 
        {
           return Results.ValidationProblem(result.Errors.ConvertToProblemDetails());
        }

        var userClaims = new List<Claim>
        {
           new Claim("EmployeeCode", employeeRequest.EmployeeCode),
           new Claim("Name", employeeRequest.Name),
           new Claim("CreatedBy", userId)
        };

        var claimResult = await userManager.AddClaimsAsync(newUser, userClaims).ConfigureAwait(false);

        if(!claimResult.Succeeded)
        {
           return Results.ValidationProblem(result.Errors.ConvertToProblemDetails());
        }

        return Results.Created($"/employees/{newUser.Id}", newUser.Id);
    }
}