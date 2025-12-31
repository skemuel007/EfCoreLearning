using EfCoreLearning.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCoreLearning.db;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        // Table mapping
        builder.ToTable("orderItems", "dbo");

        // Primary Key
        builder.HasKey(x => x.ItemID);

        // Column mappings
        builder.Property(x => x.ItemID).HasColumnName("itemID");
        builder.Property(x => x.OrderID).HasColumnName("orderID");
        builder.Property(x => x.ProductID).HasColumnName("productID");
        builder.Property(x => x.Quantity).HasColumnName("quantity");
        
        // Price mapping
        builder.Property(x => x.ItemPrice)
            .HasColumnName("itemPrice")
            .HasPrecision(18, 2);

        // Relationship (OrderItem belongs to an Order)
        builder.HasOne<Order>()
            .WithMany() // Or .WithMany(o => o.OrderItems)
            .HasForeignKey(x => x.OrderID)
            .OnDelete(DeleteBehavior.Cascade); // If order is deleted, items are deleted

        // Relationship (OrderItem belongs to a Product)
        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(x => x.ProductID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}