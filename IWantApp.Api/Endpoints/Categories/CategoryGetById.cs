using IWantApp.Api.Infra.Data;

using Microsoft.AspNetCore.Mvc;

namespace IWantApp.Api.Endpoints.Categories;

public sealed class CategoryGetById
{
    public static string Template => "/categories/{id:guid}";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    public static IResult Action([FromRoute] Guid id, ApplicationDbContext context) 
    {
        var category = context.Categories!.Where(c => c.Id == id)
                                          .Select(c => new CategoryResponse{ Id = c.Id, Name = c.Name, Active = c.Active })
                                          .FirstOrDefault();

        return category is not null ? Results.Ok(category) : Results.NotFound();
    }
}