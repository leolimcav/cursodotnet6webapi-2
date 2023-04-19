using Flunt.Notifications;

using IWantApp.Api.Domains.Products;

using Microsoft.EntityFrameworkCore;

namespace IWantApp.Api.Infra.Data;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}

    public DbSet<Product>? Products { get; set; }
    public DbSet<Category>? Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder builder) 
    {
        builder.Ignore<Notification>();

        builder.Entity<Product>()
                .Property(p => p.Description).IsRequired(false);
        builder.Entity<Product>()
                .Property(p => p.Description).HasMaxLength(255).IsRequired(false);

        builder.Entity<Category>()
                .Property(c => c.Name).IsRequired();
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configuration)
    {
        configuration.Properties<string>()
                     .HaveMaxLength(100);
    }
}