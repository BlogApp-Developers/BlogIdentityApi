namespace BlogIdentityApi.User.Repositories;

using System.Collections.Generic;
using BlogIdentityApi.User.Models;
using BlogIdentityApi.User.Repositories.Base;
using Dapper;
using Npgsql;

public class UserDapperRepository : IUserRepository
{
    private readonly string? connectionString;
    public UserDapperRepository(IConfiguration configuration)
    {
        this.connectionString = configuration.GetConnectionString("BlogWebApiDb");
    }

    public async Task CreateAsync(User? newUser)
    {
        using var connection = new NpgsqlConnection(this.connectionString);

        await connection.ExecuteAsync(@$"Insert into public.""AspNetUsers""(""Id"", 
                                                                 ""AvatarUrl"", 
                                                                 ""AboutMe"", 
                                                                 ""UserName"", 
                                                                 ""NormalizedUserName"", 
                                                                 ""Email"", 
                                                                 ""NormalizedEmail"", 
                                                                 ""EmailConfirmed"", 
                                                                 ""PasswordHash"", 
                                                                 ""SecurityStamp"", 
                                                                 ""ConcurrencyStamp"", 
                                                                 ""PhoneNumber"", 
                                                                 ""PhoneNumberConfirmed"", 
                                                                 ""TwoFactorEnabled"", 
                                                                 ""LockoutEnd"", 
                                                                 ""LockoutEnabled"", 
                                                                 ""AccessFailedCount"") 
                                                                        Values (@Id,
                                                                                @AvatarUrl, 
                                                                                @AboutMe, 
                                                                                @UserName, 
                                                                                @NormalizedUserName, 
                                                                                @Email, 
                                                                                @NormalizedEmail, 
                                                                                @EmailConfirmed, 
                                                                                @PasswordHash, 
                                                                                @SecurityStamp, 
                                                                                @ConcurrencyStamp, 
                                                                                @PhoneNumber, 
                                                                                @PhoneNumberConfirmed, 
                                                                                @TwoFactorEnabled, 
                                                                                @LockoutEnd, 
                                                                                @LockoutEnabled, 
                                                                                @AccessFailedCount);",
                                        new { Id = newUser?.Id,
                                              AvatarUrl = newUser?.AvatarUrl,
                                              AboutMe = newUser?.AboutMe,
                                              UserName = newUser?.UserName,
                                              NormalizedUserName = newUser?.NormalizedUserName,
                                              Email = newUser?.Email,
                                              NormalizedEmail = newUser?.NormalizedEmail,
                                              EmailConfirmed = newUser?.EmailConfirmed,
                                              PasswordHash = newUser?.PasswordHash,
                                              SecurityStamp = newUser?.SecurityStamp,
                                              ConcurrencyStamp = newUser?.ConcurrencyStamp,
                                              PhoneNumber = newUser?.PhoneNumber,
                                              PhoneNumberConfirmed = newUser?.PhoneNumberConfirmed,
                                              TwoFactorEnabled = newUser?.TwoFactorEnabled,
                                              LockoutEnd = newUser?.LockoutEnd,
                                              LockoutEnabled = newUser?.LockoutEnabled,
                                              AccessFailedCount = newUser?.AccessFailedCount
                                            });
    }

    public async Task DeleteAsync(Guid? id)
    {
        using var connection = new NpgsqlConnection(this.connectionString);

        var placesIds = await connection.QueryAsync<Guid?>("Select \"Id\" From public.\"AspNetUsers\"");

        var containsId = placesIds.Contains(id.Value);

        if (containsId)
        {
            await connection.ExecuteAsync("DELETE FROM public.\"AspNetUsers\" u WHERE u.\"Id\" = @Id", new { Id = id });
        }
    }

    public async Task<IEnumerable<User>> GetFiveRandomThroughTopics(Guid id)
    {
        using var connection = new NpgsqlConnection(this.connectionString);

        var topics = await connection.QueryAsync<int>(@$"Select ""TopicId"" From ""UserTopics""
                                                    Where ""UserId"" = @Id", new { Id = id });

        string topicsStr = "";
        topicsStr = topicsStr.Insert(topicsStr.Length, string.Join(',', topics));

        var userIds = await connection.QueryAsync<Guid>(@$"Select DISTINCT ""UserId"" From ""UserTopics""
                                                    Where ""TopicId"" in ({topicsStr}) and ""UserId"" not in (@Id)
                                                    LIMIT 100", new { Id = id });

        List<string> userIdsPlainStr = [];

        foreach (var userId in userIds)
        {
            userIdsPlainStr.Add("'" + userId.ToString() + "'");
        }

        string userIdsStr = "";
        userIdsStr = userIdsStr.Insert(userIdsStr.Length, string.Join(',', userIdsPlainStr));

        var users = await connection.QueryAsync<User>(@$"Select * From ""AspNetUsers""
                                                    Where ""Id"" in ({userIdsStr})");

        List<User> limitedUsers = [];
        List<int> userPlaces = [];

        var neededNum = 5;

        if (users.Count() < neededNum)
        {
            neededNum = users.Count();
        }

        while (limitedUsers.Count() < neededNum)
        {
            var userPlace = Random.Shared.Next(users.Count() - 1);

            if (!userPlaces.Contains(userPlace))
            {
                limitedUsers.Add(users.ElementAt(userPlace));
                userPlaces.Add(userPlace);
            }
        }

        return limitedUsers;
    }

    public async Task<long> UpdateAsync(User? user)
    {
        using var connection = new NpgsqlConnection(this.connectionString);

        return await connection.ExecuteAsync($@"Update public.""AspNetUsers""
                                                Set ""AvatarUrl"" = @AvatarUrl,
                                                    ""AboutMe"" = @AboutMe,
                                                    ""UserName"" = @UserName,
                                                    ""NormalizedUserName"" = @NormalizedUserName,
                                                    ""Email"" = @Email,
                                                    ""NormalizedEmail"", 
                                                    ""EmailConfirmed"" = @EmailConfirmed,
                                                    ""PasswordHash"" = @PasswordHash,
                                                    ""SecurityStamp"" = @SecurityStamp,
                                                    ""ConcurrencyStamp"" = @ConcurrencyStamp,
                                                    ""PhoneNumber"" = @PhoneNumber,
                                                    ""PhoneNumberConfirmed"" = @PhoneNumberConfirmed,
                                                    ""TwoFactorEnabled"" = @TwoFactorEnabled,
                                                    ""LockoutEnd"" = @LockoutEnd,
                                                    ""LockoutEnabled"" = @LockoutEnabled,
                                                    ""AccessFailedCount"" = @AccessFailedCount
                                                Where public.""AspNetUsers"".""Id"" = @Id", new { AvatarUrl = user?.AvatarUrl, 
                                                                                AboutMe = user?.AboutMe, 
                                                                                UserName = user?.UserName, 
                                                                                NormalizedUserName = user?.NormalizedUserName, 
                                                                                Email = user?.Email,
                                                                                EmailConfirmed = user?.EmailConfirmed,
                                                                                PasswordHash = user?.PasswordHash,
                                                                                SecurityStamp = user?.SecurityStamp,
                                                                                ConcurrencyStamp = user?.ConcurrencyStamp,
                                                                                PhoneNumber = user?.PhoneNumber,
                                                                                PhoneNumberConfirmed = user?.PhoneNumberConfirmed,
                                                                                TwoFactorEnabled = user?.TwoFactorEnabled,
                                                                                LockoutEnd = user?.LockoutEnd,
                                                                                LockoutEnabled = user?.LockoutEnabled,
                                                                                AccessFailedCount = user?.AccessFailedCount
                                                                                 });
    }
}