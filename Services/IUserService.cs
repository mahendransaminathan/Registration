
public interface IUserService
{
    Task<User> GetUserByEmail(string email);
    Task AddUser(User user);
}