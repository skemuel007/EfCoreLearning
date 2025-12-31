using EfCoreLearning.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCoreLearning.db;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        // Table mapping
        builder.ToTable("addresses", "dbo");

        // Primary Key
        builder.HasKey(x => x.AddressID);

        // Column mappings
        builder.Property(x => x.AddressID).HasColumnName("addressID");
        builder.Property(x => x.CustomerID).HasColumnName("customerID");
        builder.Property(x => x.City).HasColumnName("city");
        builder.Property(x => x.State).HasColumnName("state");
        builder.Property(x => x.Disabled).HasColumnName("disabled");

        // Relationship (Foreign Key)
        builder.HasOne<Customer>()
            .WithMany() // Or .WithMany(c => c.Addresses) if your Customer class has a collection
            .HasForeignKey(x => x.CustomerID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}