using IWantApp.Api.Domains.Products;
using IWantApp.Api.Infra.Data;

namespace IWantApp.Api.Endpoints.Categories;

public sealed class CategoryPost
{
    public static string Template => "/categories";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    public static IResult Action(CategoryRequest categoryRequest, ApplicationDbContext context) 
    {
        var category = new Category
        {
            Name = categoryRequest.Name,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = "Test",
            UpdatedBy = "Test"
        };

        context.Add<Category>(category);
        context.SaveChanges();

        return Results.Created($"/categories/{category.Id}", category.Id);
    }
}