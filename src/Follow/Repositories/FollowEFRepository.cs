namespace BlogIdentityApi.Follow.Repositories;

using System.Threading.Tasks;
using BlogIdentityApi.Base.Methods;
using BlogIdentityApi.Data;
using BlogIdentityApi.Follow.Models;
using BlogIdentityApi.Follow.Repositories.Base;
using Microsoft.EntityFrameworkCore;

public class FollowEFRepository : IFollowRepository
{
    private readonly BlogIdentityDbContext dbContext;

    public FollowEFRepository(BlogIdentityDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task CreateAsync(Follow follow)
    {
        await dbContext.Followers.AddAsync(follow);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Follow follow)
    {
        dbContext.Followers.Remove(follow);
        await dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<Follow>> GetsByIdAsync(Guid id)
    {
        var follows = this.dbContext.Followers.AsEnumerable().Where(f => f.FollowingId == id);
        return follows;
    }

    public async Task<Follow> GetByIdAsync(Guid id)
    {
        var follow = await this.dbContext.Followers.FirstOrDefaultAsync(f => f.Id == id);
        return follow;
    }
}