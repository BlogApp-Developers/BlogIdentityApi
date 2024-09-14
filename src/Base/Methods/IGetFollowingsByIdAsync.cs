namespace BlogIdentityApi.Base.Methods;

public interface IGetFollowingsByIdAsync<TEntity>
{
    public Task<IEnumerable<TEntity>> GetFollowingsByIdAsync(Guid id);
}
