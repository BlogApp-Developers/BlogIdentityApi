namespace BlogIdentityApi.RefreshToken.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BlogIdentityApi.RefreshToken.Entity;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder
            .HasKey(rt => rt.Token);

        builder
            .HasOne(rt => rt.User)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}
