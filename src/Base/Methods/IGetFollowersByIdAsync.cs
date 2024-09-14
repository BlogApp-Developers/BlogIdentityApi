namespace BlogIdentityApi.Base.Methods;

public interface IGetFollowersByIdAsync<TEntity>
{
    public Task<IEnumerable<TEntity>> GetFollowersByIdAsync(Guid id);
}
