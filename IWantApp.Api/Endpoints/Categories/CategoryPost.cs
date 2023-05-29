using System.Security.Claims;

using IWantApp.Api.Domains.Products;
using IWantApp.Api.Infra.Data;

using Microsoft.AspNetCore.Authorization;

namespace IWantApp.Api.Endpoints.Categories;

public sealed class CategoryPost
{
    public static string Template => "/categories";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "EmployeePolicy")]
    public static async Task<IResult> Action(CategoryRequest categoryRequest, HttpContext http, ApplicationDbContext context) 
    {
        var userId = http.User.Claims.First(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;
        var category = new Category(categoryRequest.Name, userId, userId);

        if(!category.IsValid)
        { 
            return Results.ValidationProblem(category.Notifications.ConvertToProblemDetails());
        }

        await context.AddAsync<Category>(category).ConfigureAwait(false);
        await context.SaveChangesAsync().ConfigureAwait(false);

        return Results.Created($"/categories/{category.Id}", category.Id);
    }
}