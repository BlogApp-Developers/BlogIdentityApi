namespace BlogIdentityApi.Data;

using BlogIdentityApi.RefreshToken.Data.Configurations;
using BlogIdentityApi.User.Data.Configurations;
using BlogIdentityApi.User.Models;
using BlogIdentityApi.RefreshToken.Entity;
using BlogIdentityApi.Follow.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BlogIdentityApi.Role.Models;
using BlogIdentityApi.Follow.Data.Configurations;

public class BlogIdentityDbContext : IdentityDbContext<User, Role, Guid>
{
    public BlogIdentityDbContext(DbContextOptions<BlogIdentityDbContext> options)
        : base(options) {}

    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Follow> Followers { get;}

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new UserConfiguration());
        builder.ApplyConfiguration(new RefreshTokenConfiguration());
        builder.ApplyConfiguration(new FollowConfiguration());
    }
}