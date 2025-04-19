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
builder.Services.AddScoped<IUserService, UserService>();


builder.Build().Run();
