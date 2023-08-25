using IWantApp.Api.Infra.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace IWantApp.Api.Endpoints.Products;

public sealed class ProductGetShowcase
{
    public static string Template => "/products/showcase";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    [AllowAnonymous()]
    public static async Task<IResult> Action(ApplicationDbContext dbContext, int page = 1, int rows = 10, string orderBy = "name", CancellationToken cancellationToken = default)
    {
        var querybase = dbContext.Products!
                                 .Include(p => p.Category)
                                 .Where(p => p.Category!.Active && p.HasStock);

        if(orderBy.Equals("name"))
        {
            querybase = querybase.OrderBy(p => p.Name);
        } else
        {
            querybase = querybase.OrderBy(p => p.Price);
        }

        var queryFilter = querybase.Skip((page -1) * rows).Take(rows);

        var products = await queryFilter.ToListAsync(cancellationToken).ConfigureAwait(false);
        var results = products.Select(p => new ProductResponse(p.Name!, p.Category!.Name!, p.Description!, p.HasStock, p.Price, p.Active));

        return Results.Ok(results);
    }
}