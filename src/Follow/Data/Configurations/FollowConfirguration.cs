namespace BlogIdentityApi.Follow.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BlogIdentityApi.Follow.Models;

public class FollowConfiguration : IEntityTypeConfiguration<Follow>
{
    public void Configure(EntityTypeBuilder<Follow> builder)
    {
        builder
            .HasKey(f => f.Id);

        builder
            .HasOne(f => f.Following)
            .WithMany(f => f.Followers)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}
