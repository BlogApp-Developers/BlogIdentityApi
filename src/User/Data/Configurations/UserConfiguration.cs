namespace BlogIdentityApi.User.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BlogIdentityApi.User.Models;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasIndex(u => u.Email).IsUnique();
        builder.Property(u => u.UserName).IsRequired();
        builder.Property(e => e.AboutMe).HasMaxLength(300);
        builder.Property(e => e.UserName).IsRequired().HasMaxLength(30);
        builder.Property(e => e.AvatarUrl).HasDefaultValue("https://st3.depositphotos.com/9998432/13335/v/450/depositphotos_133352156-stock-illustration-default-placeholder-profile-icon.jpg");
    }
}
