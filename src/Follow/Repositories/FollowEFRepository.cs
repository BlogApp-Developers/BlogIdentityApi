namespace BlogIdentityApi.Follow.Repositories;

using System.Threading.Tasks;
using BlogIdentityApi.Data;
using BlogIdentityApi.Follow.Models;
using BlogIdentityApi.Follow.Repositories.Base;
using BlogIdentityApi.User.Models;
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
        var followings = this.dbContext.Followers.AsEnumerable().Where(f => f.FollowingId == id);
        return followings;
    }

    public async Task<Follow> GetByIdAsync(Guid personalId, Guid userId)
    {
        var follow = await this.dbContext.Followers.FirstOrDefaultAsync(f => f.FollowingId == personalId && f.FollowerId == userId);
        return follow;
    }

    public async Task<IEnumerable<Follow>> GetsInvertByIdAsync(Guid id)
    {
        var followers = this.dbContext.Followers.AsEnumerable().Where(f => f.FollowerId == id);
        return followers;
    }

    public async Task<IEnumerable<User>> GetFollowersByIdAsync(Guid id)
    {
        var followers = await this.GetsInvertByIdAsync(id);
        var followersAccounts = new List<User>();

        foreach (var follower in followers)
        {
            followersAccounts.Add(dbContext.Users.First(u => u.Id == follower.FollowerId));
        }

        return followersAccounts;
    }

    public async Task<IEnumerable<User>> GetFollowingsByIdAsync(Guid id)
    {
        var followings = await this.GetsByIdAsync(id);
        var followingsAccounts = new List<User>();

        foreach (var following in followings)
        {
            followingsAccounts.Add(dbContext.Users.First(u => u.Id == following.FollowingId));
        }

        return followingsAccounts;
    }

    public bool IsFollowing(Guid personalId, Guid userId)
    {
        var follower = this.dbContext.Followers.FirstOrDefault(f => f.FollowingId == personalId && f.FollowerId == userId);

        if (follower != null)
        {
            return true;
        }
        return false;
    }
}