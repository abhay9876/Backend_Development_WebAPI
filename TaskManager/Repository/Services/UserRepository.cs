using TaskManager.Models.Entity;
using TaskManager.Repository.Context;
using TaskManager.Repository.Interface;

namespace TaskManager.Repository.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly TaskManagerDbContext _context;

        public UserRepository(TaskManagerDbContext context)
        {
            _context = context;
        }


        public User Register(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();  //---> 
            return user;
        }


        public User Login(string username, string password)
        {
            return _context.Users.FirstOrDefault(n => n.UserName == username && n.Password == password);
        }


        public User GetUserByUserName(string username)
        {
            return _context.Users.FirstOrDefault(n => n.UserName == username);
        }

        public User Update(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
            return user;
        }
    }
}
