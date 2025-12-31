using EfCoreLearning.models;
using Microsoft.EntityFrameworkCore;

namespace EfCoreLearning.db;

public class EshopDbContext : DbContext
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    
    public EshopDbContext(DbContextOptions<EshopDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // setting decimal precision
        /*foreach (var property in modelBuilder.Model.GetEntityTypes()
                     .SelectMany(t => t.GetProperties())
                     .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
        {
            property.SetPrecision(10);
            property.SetScale(4);
        }*/
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EshopDbContext).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<decimal>().HavePrecision(10, 4);
        base.ConfigureConventions(configurationBuilder);
    }
}