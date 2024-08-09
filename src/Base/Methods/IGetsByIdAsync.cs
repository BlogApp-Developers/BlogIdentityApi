namespace BlogIdentityApi.Base.Methods;

public interface IGetsByIdAsync<TEntity>
{
    public Task<IEnumerable<TEntity>> GetsByIdAsync(Guid id);
}
