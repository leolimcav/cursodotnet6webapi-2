using Flunt.Notifications;

using IWantApp.Api.Domains.Products;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IWantApp.Api.Infra.Data;

public sealed class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}

    public DbSet<Product>? Products { get; set; }

    public DbSet<Category>? Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder builder) 
    {
        base.OnModelCreating(builder);

        builder.Ignore<Notification>();

        builder.Entity<Product>()
                .Property(p => p.Description).HasMaxLength(255).IsRequired(false);

        builder.Entity<Product>()
               .Property(p => p.Price).HasColumnType("decimal(10, 2)").IsRequired();

        builder.Entity<Category>()
                .Property(c => c.Name).IsRequired();
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configuration)
    {
        configuration.Properties<string>()
                     .HaveMaxLength(100);
    }
}