using EfCoreLearning.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCoreLearning.db;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        // Table mapping
        builder.ToTable("categories", "dbo");

        // Primary Key
        builder.HasKey(x => x.CategoryID);

        // Column mappings
        builder.Property(x => x.CategoryID).HasColumnName("categoryID");
        builder.Property(x => x.CategoryName).HasColumnName("categoryName").IsRequired();
    }
}