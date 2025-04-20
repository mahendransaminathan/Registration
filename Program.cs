using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Registration.Services;
using Registration.Repositories;
using Registration.Data;
using Microsoft.Extensions.DependencyInjection;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();


 if (builder.Configuration["DatabaseString:SelectedDatabase"] == "CosmosDB")
 {
    builder.Services.AddSingleton<CosmosClient>(serviceProvider => 
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var cosmosEndPoint = Environment.GetEnvironmentVariable("COSMOS_DB_ENDPOINT");
        var cosmosKey = Environment.GetEnvironmentVariable("COSMOS_DB_KEY");    

        if (string.IsNullOrEmpty(cosmosKey))
        {
            throw new ArgumentNullException("COSMOS_DB_KEY", "❌ Cosmos DB Key is missing");
        }
        return new CosmosClient(cosmosEndPoint, cosmosKey);
    }
    );
    builder.Services.AddScoped<IUserRepository, UserCosmosRepository>();
 }
 else
 {
        builder.Services.AddDbContext<ApplicationDBContext>(options =>
        {
            var connectionString = builder.Configuration["ConnectionStrings:SqlConnectionString"];
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("The SQL connection string is not configured.");
            }
            options.UseSqlServer(connectionString);
        });
        builder.Services.AddScoped<IUserRepository, UserRepository>();
 }
builder.Services.AddScoped<IUserService, UserService>();


builder.Build().Run();
