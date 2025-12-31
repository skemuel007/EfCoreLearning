using EfCoreLearning.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCoreLearning.db;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // Table mapping
        builder.ToTable("products", "dbo");

        // Primary Key
        builder.HasKey(x => x.ProductID);

        // Column mappings
        builder.Property(x => x.ProductID).HasColumnName("productID");
        builder.Property(x => x.CategoryID).HasColumnName("categoryID");
        builder.Property(x => x.ProductName).HasColumnName("productName").IsRequired();
        
        // Price mapping with precision
        builder.Property(x => x.ListPrice)
            .HasColumnName("listPrice")
            .HasPrecision(18, 2);

        // Relationship (Foreign Key)
        builder.HasOne<Category>()
            .WithMany() // Or .WithMany(c => c.Products) if Category has a List<Product>
            .HasForeignKey(x => x.CategoryID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}