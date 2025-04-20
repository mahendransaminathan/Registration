using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
// using Newtonsoft.Json;
using System.Text.Json;

namespace Regis.Function
{
    public class Registration
    {
        private readonly ILogger<Registration> _logger;

        private readonly IUserService _userService;

        public Registration(ILogger<Registration> logger, IUserService userService)
        {
            _userService = userService;        
            _logger = logger;
        }

        [Function("Registration")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            // Parse the request body to get the user details
            if (req.Method == HttpMethods.Get)
            {
                // Handle GET request
                var email = req.Query["email"];

                if (string.IsNullOrEmpty(email))
                {
                    return new BadRequestObjectResult("Email is required.");
                }

                var user = _userService.GetUserByEmail(email);
                if (user == null)
                {
                    return new NotFoundObjectResult("User not found.");
                }
                return new OkObjectResult(user);
                
            }
            else if (req.Method == HttpMethods.Post)
            {
                // Handle POST request
                var requestBody = new StreamReader(req.Body).ReadToEnd();
                var user = JsonSerializer.Deserialize<User>(requestBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (user == null || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
                {
                    return new BadRequestObjectResult("Invalid user data.");
                }

                _userService.AddUser(user);
                return new CreatedResult($"/users/{user.Id}", user);
            }


            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
