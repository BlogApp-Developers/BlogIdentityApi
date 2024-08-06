namespace BlogIdentityApi.Base.Methods;

public interface IDeleteAsync<TEntity>
{
    public Task DeleteAsync(TEntity entity);
}
