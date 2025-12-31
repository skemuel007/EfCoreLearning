using EfCoreLearning.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCoreLearning.db;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        // Table mapping
        builder.ToTable("customers", "dbo");

        // Primary Key
        builder.HasKey(x => x.CustomerID);

        // Column mappings
        builder.Property(x => x.CustomerID).HasColumnName("customerID");
        builder.Property(x => x.EmailAddress).HasColumnName("emailAddress").IsRequired();
        builder.Property(x => x.FirstName).HasColumnName("firstName").IsRequired();
        builder.Property(x => x.LastName).HasColumnName("lastName").IsRequired();
        builder.Property(x => x.ShipAddressID).HasColumnName("shipAddressID");
        builder.Property(x => x.BillingAddressID).HasColumnName("billingAddressID");
    }
}