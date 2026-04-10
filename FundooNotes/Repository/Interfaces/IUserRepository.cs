using FundooNotes.Models.Entity;

namespace FundooNotes.Repository.Interfaces
{
    public interface IUserRepository
    {
        User Register(User user);
        User Login(string email, string password);
        User GetUserByEmail(string email);

        User GetUserByID(int Id);
        void UpdateUser(User user);
        User GetUserByResetToken(string token);

    }
}
