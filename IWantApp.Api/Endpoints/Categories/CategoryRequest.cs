namespace IWantApp.Api.Endpoints.Categories;

public sealed class CategoryRequest
{
    public string? Name { get; set; }
    public bool Active { get; set; }
}