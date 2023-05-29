using Flunt.Validations;

namespace IWantApp.Api.Domains.Products;

public sealed class Category : Entity
{
    public Category(string? name, string? createdBy, string? updatedBy)
    { 
        Name = name;
        Active = true;
        CreatedBy = createdBy;
        UpdatedBy = updatedBy;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        Validate();
    }

    public bool Active { get; set; }

    public void UpdateInfo(string? name, bool active, string updatedBy)
    {
        Name = name;
        Active = active;
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;

        Validate();
    }

    public void Validate()
    {
        var contract = new Contract<Category>()
            .IsNotNullOrEmpty(Name, "Name")
            .IsGreaterOrEqualsThan(Name, 3,"Name")
            .IsNotNullOrEmpty(CreatedBy, "CreatedBy")
            .IsNotNullOrEmpty(UpdatedBy, "UpdatedAt");

        AddNotifications(contract);
    }
}