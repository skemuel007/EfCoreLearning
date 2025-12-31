using EfCoreLearning.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCoreLearning.db;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        // Table mapping
        builder.ToTable("orders", "dbo");

        // Primary Key
        builder.HasKey(x => x.OrderID);

        // Column mappings
        builder.Property(x => x.OrderID).HasColumnName("orderID");
        builder.Property(x => x.CustomerID).HasColumnName("customerID");
        builder.Property(x => x.OrderDate).HasColumnName("orderDate");
        
        // Money/Amount mapping
        builder.Property(x => x.ShipAmount)
            .HasColumnName("shipAmount")
            .HasPrecision(18, 2);

        builder.Property(x => x.TaxAmount)
            .HasColumnName("taxAmount")
            .HasPrecision(18, 2);

        // Relationship (Order belongs to a Customer)
        builder.HasOne<Customer>()
            .WithMany() // Or .WithMany(c => c.Orders)
            .HasForeignKey(x => x.CustomerID);
    }
}