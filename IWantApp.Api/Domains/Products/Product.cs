namespace IWantApp.Api.Domains.Products;

public sealed class Product : Entity
{
    public string? Description { get; set; }
    public bool HasStock { get; set; }
    public Guid? CategoryId { get; set; }
    public Category? Category { get; set; }
    public bool Active { get; set; } = true;
}