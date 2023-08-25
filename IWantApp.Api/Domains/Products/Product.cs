using Flunt.Validations;

namespace IWantApp.Api.Domains.Products;

public sealed class Product : Entity
{ 
    public string? Description { get; private set; }
    public bool HasStock { get; private set; }
    public Guid? CategoryId { get; private set; }
    public Category? Category { get; private set; }
    public bool Active { get; private set; } = true;
    public decimal Price { get; private set; }

    public Product(string name, Category category, string description, bool hasStock, decimal price, string createdBy)
    {
        Name = name;
        Category = category;
        Description = description;
        HasStock = hasStock;
        Price = price;

        CreatedBy = createdBy;
        UpdatedBy = createdBy;
        CreatedAt = DateTime.Now;
        UpdatedAt = DateTime.Now;

        Validate();
    }

    private Product() { }

    private void Validate()
    {
        var contract = new Contract<Product>()
            .IsNotNullOrEmpty(Name, "Name")
            .IsGreaterOrEqualsThan(Name, 3, "Name")
            .IsNotNull(Category, "Category", "Category not found")
            .IsNotNullOrEmpty(Description, "Description")
            .IsGreaterOrEqualsThan(Description, 3, "Description")
            .IsGreaterOrEqualsThan(Price, 1, "Price")
            .IsNotNullOrEmpty(CreatedBy, "CreatedBy")
            .IsNotNullOrEmpty(UpdatedBy, "UpdatedBy");

        AddNotifications(contract);
    }
}