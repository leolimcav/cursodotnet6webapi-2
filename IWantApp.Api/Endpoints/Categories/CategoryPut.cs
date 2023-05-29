using System.Security.Claims;

using IWantApp.Api.Infra.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IWantApp.Api.Endpoints.Categories;

public sealed class CategoryPut
{
    public static string Template => "/categories/{id:guid}";
    public static string[] Methods => new string[] { HttpMethod.Put.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "EmployeePolicy")]
    public static async Task<IResult> Action([FromRoute] Guid id, CategoryRequest categoryRequest, ApplicationDbContext context, HttpContext http) 
    {
        var userId = http.User.Claims.First(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;
        var category = await context.Categories!.FirstOrDefaultAsync(c => c.Id == id).ConfigureAwait(false);

        if(category is null) {
            return Results.NotFound();
        }

        category.UpdateInfo(categoryRequest.Name, categoryRequest.Active, userId);

        if(!category.IsValid)
        {
            return Results.ValidationProblem(category.Notifications.ConvertToProblemDetails());
        }

        await context.SaveChangesAsync().ConfigureAwait(false);

        return Results.Ok(category.Id);
    }
}