namespace BlogIdentityApi.User.Repositories;

using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using BlogIdentityApi.Data;
using BlogIdentityApi.Enums;
using BlogIdentityApi.Options;
using BlogIdentityApi.User.Models;
using BlogIdentityApi.User.Repositories.Base;
using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;
using RabbitMQ.Client;

public class UserRabbitMqRepository : IUserRepository
{
    private readonly ConnectionFactory rabbitMqConnectionFactory;
    private readonly BlogIdentityDbContext dbContext;
    private readonly string connectionString;
    public UserRabbitMqRepository(BlogIdentityDbContext dbContext, IOptionsSnapshot<RabbitMqOptions> optionsSnapshot, IConfiguration configuration)
    {
        this.rabbitMqConnectionFactory = new ConnectionFactory() {
                HostName = optionsSnapshot.Value.HostName,
                UserName = optionsSnapshot.Value.UserName,
                Password = optionsSnapshot.Value.Password
            };
        this.dbContext = dbContext;
        this.connectionString = configuration.GetConnectionString("PostgreSqlDev");
    }

    public async Task CreateAsync(User? newUser)
    {
        using var connection = this.rabbitMqConnectionFactory.CreateConnection();
        using var channel = connection.CreateModel();
        var destination = "user";
        var rabbitMqAction = RabbitMQAction.Create;

        var result = channel.QueueDeclare(
            queue: destination,
            durable: true,
            exclusive: false,
            autoDelete: false
        );

        var userJson = "\"RabbitMQAction\": " + rabbitMqAction + "&" + JsonSerializer.Serialize(newUser);

        var messageInBytes = Encoding.Unicode.GetBytes(userJson);

        channel.BasicPublish(
            exchange: string.Empty,
            routingKey: destination,
            basicProperties: null,
            body: messageInBytes
        );
    }

    public async Task DeleteAsync(Guid? id)
    {
        var deletingUser = this.dbContext.Users.FirstOrDefault(u => u.Id == id);

        if (deletingUser == null)
        {
            throw new ArgumentNullException(nameof(id), $"Invalid {id} of a user!");
        }

        using var connection = this.rabbitMqConnectionFactory.CreateConnection();
        using var channel = connection.CreateModel();
        var destination = "user";
        var rabbitMqAction = RabbitMQAction.Delete;

        var result = channel.QueueDeclare(
            queue: destination,
            durable: true,
            exclusive: false,
            autoDelete: false
        );

        var userJson = "\"RabbitMQAction\": " + rabbitMqAction + "\n" + JsonSerializer.Serialize(deletingUser);

        var messageInBytes = Encoding.Unicode.GetBytes(userJson);

        channel.BasicPublish(
            exchange: string.Empty,
            routingKey: destination,
            basicProperties: null,
            body: messageInBytes
        );
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

    public async Task UpdateAsync(User? user)
    {
        using var connection = this.rabbitMqConnectionFactory.CreateConnection();
        using var channel = connection.CreateModel();
        var destination = "user";
        var rabbitMqAction = RabbitMQAction.Update;

        var result = channel.QueueDeclare(
            queue: destination,
            durable: true,
            exclusive: false,
            autoDelete: false
        );

        var userJson = "\"RabbitMQAction\": " + rabbitMqAction + "\n" + JsonSerializer.Serialize(user);

        var messageInBytes = Encoding.Unicode.GetBytes(userJson);

        channel.BasicPublish(
            exchange: string.Empty,
            routingKey: destination,
            basicProperties: null,
            body: messageInBytes
        );
    }
}