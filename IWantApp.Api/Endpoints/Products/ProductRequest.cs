namespace IWantApp.Api.Endpoints.Products;

public sealed record ProductRequest(Guid CategoryId, string Name, string Description, decimal Price, bool HasStock, bool Active);