using FundooNotes.Models.Entity;
using FundooNotes.Repository.Context;
using FundooNotes.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FundooNotes.Repository.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext context;

        public UserRepository(AppDbContext context)
        {
            this.context = context;
        }

        public User Register(User user)
        {
            context.Users.Add(user);
            context.SaveChanges();
            return user;
        }

        public User Login(string email,string password)
        {
            return context.Users.FirstOrDefault(x => x.Email == email && x.Password==password);
        }

        public User GetUserByEmail(string email)
        {
            return context.Users.FirstOrDefault(x => x.Email == email);
        }
        public void UpdateUser(User user)
        {
            context.Users.Update(user);
            context.SaveChanges();
        }

        public User GetUserByResetToken(string token)
        {
            return context.Users.FirstOrDefault(u => u.ResetToken == token);
        }

        public User GetUserByID(int Id)
        {
            return context.Users.FirstOrDefault(u => u.Id == Id);
        }
    }
}
