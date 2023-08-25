using IWantApp.Api.Infra.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace IWantApp.Api.Endpoints.Products;

public sealed class ProductGetById
{
    public static string Template => "/products/{productId:guid}";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "EmployeePolicy")]
    public static async Task<IResult> Action(Guid productId, ApplicationDbContext dbContext, CancellationToken cancellationToken = default)
    {
        var product = await dbContext.Products!.Include(p => p.Category).FirstOrDefaultAsync((p => p.Id == productId), cancellationToken).ConfigureAwait(false);

        var result = new ProductResponse(product!.Name!, product.Category!.Name!, product.Description!, product.HasStock, product.Price, product.Active);

        return Results.Ok(result);
    }
}