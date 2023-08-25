namespace IWantApp.Api.Endpoints.Products;

public sealed record ProductResponse(string Name, string CategoryName, string Description, bool HasStock, decimal price, bool Active);