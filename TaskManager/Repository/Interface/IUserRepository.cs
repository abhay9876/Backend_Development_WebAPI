using TaskManager.Models.Entity;

namespace TaskManager.Repository.Interface
{
    public interface IUserRepository
    {

        User Register(User user);
        User Login(string username, string password);
        User? GetUserByUserName(string username);
        User Update(User user);

    }
}
