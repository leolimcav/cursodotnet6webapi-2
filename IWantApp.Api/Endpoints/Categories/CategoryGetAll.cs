using IWantApp.Api.Infra.Data;

using Microsoft.EntityFrameworkCore;

namespace IWantApp.Api.Endpoints.Categories;

public sealed class CategoryGetAll
{
    public static string Template => "/categories";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    public static async Task<IResult> Action(ApplicationDbContext context) 
    {
        var categories = await context.Categories!
            .Select(c => new CategoryResponse{ Id = c.Id, Name = c.Name, Active = c.Active })
            .ToListAsync().ConfigureAwait(false);

        return Results.Ok(categories);
    }
}