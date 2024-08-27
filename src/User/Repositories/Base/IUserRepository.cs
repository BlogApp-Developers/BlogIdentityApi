namespace BlogIdentityApi.User.Repositories.Base;

using BlogIdentityApi.User.Models;

public interface IUserRepository
{
    Task CreateAsync(User? newUser);
    Task DeleteAsync(Guid? id);
    Task<long> UpdateAsync(User? user);
    Task<IEnumerable<User>> GetFiveRandomThroughTopics(User user);
}