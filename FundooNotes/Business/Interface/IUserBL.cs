using FundooNotes.Models.DTOs;
using FundooNotes.Models.Entity;

namespace FundooNotes.Business.Interface
{
    public interface IUserBL
    {
        User Register(RegisterDTO registerDto);
        (string token, User user) Login(LoginDTO loginDto);
        void ForgotPassword(ForgotPasswordDTO forgotPasswordDto);
        bool ResetPassword(ResetPasswordDTO resetPasswordDto);

        User GetUserByID(int Id);
    }
}
