using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace IWantApp.Api.Endpoints.Clients;

public sealed class ClientPost
{
    public static string Template => "/clients";

    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };

    public static Delegate Handle => Action;

    [AllowAnonymous]
    public static async Task<IResult> Action(ClientRequest clientRequest, UserManager<IdentityUser> userManager)
    {
       
        var newUser =  new IdentityUser 
        {
            UserName = clientRequest.Email,
            Email = clientRequest.Email
        };
        var result = await userManager.CreateAsync(newUser, clientRequest.Password).ConfigureAwait(false);

        if(!result.Succeeded) 
        {
           return Results.ValidationProblem(result.Errors.ConvertToProblemDetails());
        }

        var userClaims = new List<Claim>
        {
           new Claim("Cpf", clientRequest.Cpf),
           new Claim("Name", clientRequest.Name),
        };

        var claimResult = await userManager.AddClaimsAsync(newUser, userClaims).ConfigureAwait(false);

        if(!claimResult.Succeeded)
        {
           return Results.ValidationProblem(result.Errors.ConvertToProblemDetails());
        }

        return Results.Created($"/employees/{newUser.Id}", newUser.Id);
    }
}