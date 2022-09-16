using IWantApp.Api.Infra.Data;

using Microsoft.AspNetCore.Mvc;

namespace IWantApp.Api.Endpoints.Categories;

public sealed class CategoryPut
{
    public static string Template => "/categories/{id}";
    public static string[] Methods => new string[] { HttpMethod.Put.ToString() };
    public static Delegate Handle => Action;

    public static IResult Action([FromRoute] Guid id, CategoryRequest categoryRequest, ApplicationDbContext context) 
    {
        var category = context.Categories!.FirstOrDefault(c => c.Id == id);

        if(category is null) {
            return Results.NotFound();
        }

        category.Name = categoryRequest.Name;
        category.Active = categoryRequest.Active;
        category.UpdatedAt = DateTime.UtcNow;
        category.UpdatedBy = "Test";

        context.SaveChanges();

        return Results.Ok(category.Id);
    }
}