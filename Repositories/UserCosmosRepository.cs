
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;

public class UserCosmosRepository : IUserRepository
{
    private readonly CosmosClient _cosmosClient;
    private readonly Container _container;
    private readonly Database _database;

    public UserCosmosRepository(CosmosClient cosmosClient, IConfiguration configuration)
    {
        _cosmosClient = cosmosClient;
        _database = _cosmosClient.GetDatabase(configuration["CosmosDb:DatabaseName"]);
        _container = _database.GetContainer(configuration["CosmosDb:ContainerName"]);
    }

    public async Task<User> GetUserByEmail(string email)
    {
        var query = new QueryDefinition("SELECT * FROM user WHERE c.Email = @Email")
            .WithParameter("@Email", email);

        using var iterator = _container.GetItemQueryIterator<User>(query);
        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            return response.FirstOrDefault(); // Return the first matching user
        }

        return null; // Return null if no user is found
    }
    public async Task AddUser(User user)
    {
        try
        {
            await _container.CreateItemAsync(user, new PartitionKey(user.Id));
        }
        catch (CosmosException ex)
        {
            // Log or handle the exception as needed
            Console.WriteLine($"Cosmos DB error: {ex.StatusCode} - {ex.Message}");
            throw;
        }
    }
}