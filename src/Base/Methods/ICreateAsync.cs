namespace BlogIdentityApi.Base.Methods;

public interface ICreateAsync<TEntity>
{
    public Task CreateAsync(TEntity entity);
}
