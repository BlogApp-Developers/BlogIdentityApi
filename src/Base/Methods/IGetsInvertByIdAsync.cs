namespace BlogIdentityApi.Base.Methods;

public interface IGetsInvertByIdAsync<TEntity>
{
    public Task<IEnumerable<TEntity>> GetsInvertByIdAsync(Guid id);
}
