using System.Security.Claims;

using IWantApp.Api.Domains.Products;
using IWantApp.Api.Infra.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace IWantApp.Api.Endpoints.Products;

public sealed class ProductPost
{
    public static string Template => "/products";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "EmployeePolicy")]
    public static async Task<IResult> Action(ProductRequest request, HttpContext httpContext, ApplicationDbContext dbContext, CancellationToken cancellationToken = default)
    {
        var userId = httpContext.User.Claims.First(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;
        var category = await dbContext
            .Categories
            !.FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken)
            .ConfigureAwait(false);
        var product = new Product(request.Name, category!, request.Description, request.HasStock, request.Price, userId);

        if (!product.IsValid)
        {
            return Results.ValidationProblem(product.Notifications.ConvertToProblemDetails());
        }

        await dbContext.Products!.AddAsync(product, cancellationToken).ConfigureAwait(false);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Results.Created($"/products/{product.Id}", product.Id);
    }
}