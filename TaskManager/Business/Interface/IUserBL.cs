using TaskManager.Models.DTOs;
using TaskManager.Models.Entity;

namespace TaskManager.Business.Interface
{
    public interface IUserBL
    {
        User Register(RegisterDTO registerDto);
        (string token, User user) Login(LoginDTO loginDto);

    }
}
