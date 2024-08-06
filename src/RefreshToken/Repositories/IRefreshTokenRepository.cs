namespace BlogIdentityApi.RefreshToken.Repositories;

using BlogIdentityApi.Base.Methods;
using BlogIdentityApi.RefreshToken.Entity;

    public interface IRefreshTokenRepository : IDeleteAsync<RefreshToken>, ICreateAsync<RefreshToken>
    {
        public Task<RefreshToken?> GetRefreshTokenAsync(RefreshToken refreshToken);
        public Task DeleteRangeRefreshTokensAsync(RefreshToken refreshToken);
    }
