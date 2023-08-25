using IWantApp.Api.Infra.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace IWantApp.Api.Endpoints.Products;

public sealed class ProductGetAll
{
    public static string Template => "/products";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "EmployeePolicy")]
    public static async Task<IResult> Action(ApplicationDbContext dbContext, CancellationToken cancellationToken = default)
    {
        var products = await dbContext.Products!.Include(p => p.Category).OrderBy(p => p.Name).ToListAsync(cancellationToken).ConfigureAwait(false);
        var results = products.Select(p => new ProductResponse(p.Name!, p.Category!.Name!, p.Description!, p.HasStock, p.Price ,p.Active));

        return Results.Ok(results);
    }
}