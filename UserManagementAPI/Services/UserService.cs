public interface IUserService
{
    IEnumerable<User> GetUsers();
    User GetUserById(int id);
    void CreateUser(User user);
    void UpdateUser(User user);
    void DeleteUser(int id);
}

public class UserService : IUserService
{
    private readonly List<User> _users = new List<User>();

    public IEnumerable<User> GetUsers() => _users;
    public User GetUserById(int id) => _users.FirstOrDefault(u => u.Id == id);
    public void CreateUser(User user) => _users.Add(user);
    public void UpdateUser(User user)
    {
        var existing = GetUserById(user.Id);
        if (existing != null)
        {
            existing.Name = user.Name;
            existing.Email = user.Email;
        }
    }
    public void DeleteUser(int id) => _users.RemoveAll(u => u.Id == id);
}
