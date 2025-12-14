using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipeazi.Infrastructure.src.Identity;

namespace Shipeazi.Infrastructure.src.Persistence.Configurations
{
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.Property(u => u.DisplayName)
                .HasMaxLength(100);

            builder.Property(u => u.ShipeaziAddress)
                .HasMaxLength(50);

            // Configure Address as an owned type (optional)
            builder.OwnsOne(u => u.Address, address =>
            {
                address.Property(a => a.Street)
                    .HasColumnName("AddressStreet")
                    .HasMaxLength(200);

                address.Property(a => a.City)
                    .HasColumnName("AddressCity")
                    .HasMaxLength(100);

                address.Property(a => a.State)
                    .HasColumnName("AddressState")
                    .HasMaxLength(100);

                address.Property(a => a.PostalCode)
                    .HasColumnName("AddressPostalCode")
                    .HasMaxLength(20);

                address.Property(a => a.Country)
                    .HasColumnName("AddressCountry")
                    .HasMaxLength(100);
            });

            // Configure PhoneNumber as an owned type (required)
            builder.OwnsOne(u => u.Phone, phone =>
            {
                phone.Property(p => p.CountryCode)
                    .HasColumnName("PhoneCountryCode")
                    .IsRequired()
                    .HasMaxLength(10);

                phone.Property(p => p.Value)
                    .HasColumnName("PhoneNumber")
                    .IsRequired()
                    .HasMaxLength(20);
            });
        }
    }
}
