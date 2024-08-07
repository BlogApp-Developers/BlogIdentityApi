#pragma warning disable CS1998

namespace BlogIdentityApi.RefreshToken.Repositories.Ef_Core;

using BlogIdentityApi.RefreshToken.Repositories;
using BlogIdentityApi.RefreshToken.Entity;
using BlogIdentityApi.Data;
using Microsoft.EntityFrameworkCore;

public class RefreshTokenEfCoreRepository : IRefreshTokenRepository
{
    private readonly BlogIdentityDbContext dbContext;

    public RefreshTokenEfCoreRepository(BlogIdentityDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task CreateAsync(RefreshToken refreshToken)
    {
        await dbContext.RefreshTokens.AddAsync(refreshToken);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(RefreshToken refreshToken)
    {
        dbContext.RefreshTokens.Remove(refreshToken);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteRangeRefreshTokensAsync(RefreshToken refreshToken)
    {
        var refreshTokenToDelete = dbContext.RefreshTokens.Where(rt => rt.UserId == refreshToken.UserId);
        dbContext.RefreshTokens.RemoveRange(refreshTokenToDelete);
        await dbContext.SaveChangesAsync();
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(RefreshToken refreshToken)
    {
        return await dbContext.RefreshTokens.Where(refToken => refToken.Token == refreshToken.Token && refToken.UserId == refreshToken.UserId).FirstOrDefaultAsync();
    }
}
