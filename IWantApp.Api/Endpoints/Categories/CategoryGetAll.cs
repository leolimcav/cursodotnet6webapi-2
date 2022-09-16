using IWantApp.Api.Infra.Data;

namespace IWantApp.Api.Endpoints.Categories;

public sealed class CategoryGetAll
{
    public static string Template => "/categories";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    public static IResult Action(ApplicationDbContext context) 
    {
        var categories = context.Categories!.Select(c => new CategoryResponse{ Id = c.Id, Name = c.Name, Active = c.Active }).ToList();

        return Results.Ok(categories);
    }
}