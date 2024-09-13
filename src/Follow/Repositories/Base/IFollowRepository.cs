namespace BlogIdentityApi.Follow.Repositories.Base;

using BlogIdentityApi.Base.Methods;
using BlogIdentityApi.Follow.Models;

    public interface IFollowRepository : ICreateAsync<Follow>, IDeleteAsync<Follow>, IGetsByIdAsync<Follow>, IGetByIdAsync<Follow>, IGetsInvertByIdAsync<Follow> {}
