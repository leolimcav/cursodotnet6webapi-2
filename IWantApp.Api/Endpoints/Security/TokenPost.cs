using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace IWantApp.Api.Endpoints.Security;

public sealed class TokenPost 
{
    public static string Template => "/token";

    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };

    public static Delegate Handle => Action;

    [AllowAnonymous]
    public static async Task<IResult> Action(LoginRequest request, IConfiguration configuration, UserManager<IdentityUser> userManager)
    {
        var user = await userManager.FindByEmailAsync(request.Email).ConfigureAwait(false);

        if(user is null)
        {
            return Results.BadRequest();
        }

        var isUserPasswordCorrect = await userManager.CheckPasswordAsync(user, request.Password).ConfigureAwait(false);
        if(!isUserPasswordCorrect)
        {
            return Results.BadRequest("Email/Password is wrong, check and try again!");
        }
        
        var claims = await userManager.GetClaimsAsync(user).ConfigureAwait(false);
        var subject = new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Email, request.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
        });
        subject.AddClaims(claims);

        var jwtTokenSettings = configuration.GetSection("JwtBearerTokenSettings");
        var key = Encoding.ASCII.GetBytes(jwtTokenSettings["SecretKey"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = subject,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Audience = jwtTokenSettings["Audience"],
            Issuer = jwtTokenSettings["Issuer"],
            Expires = DateTime.UtcNow.AddMinutes(int.Parse(jwtTokenSettings["ExpiryTimeInSeconds"])),
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return Results.Ok(new 
                {
                    token = tokenHandler.WriteToken(token),
                });
    }
}