using Azure.Core;
using FundooNotes.Business.Interface;
using FundooNotes.Models.DTOs;
using FundooNotes.Models.Entity;
using FundooNotes.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using BC = BCrypt.Net.BCrypt;

namespace FundooNotes.Business.Services
{
    public class UserBL : IUserBL
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        

        public UserBL(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public User Register(RegisterDTO registerDto)
        {
            // check if user already exists..
            var existingUser = _userRepository.GetUserByEmail(registerDto.Email);
            if (existingUser != null)
                throw new Exception("User with this email already exists.");

            var user = new User
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                Password = BC.HashPassword(registerDto.Password),
                CreatedAt = DateTime.UtcNow
            };

            return _userRepository.Register(user);
        }

        public (string token,User user) Login(LoginDTO loginDto)
        {
            var user = _userRepository.GetUserByEmail(loginDto.Email);
            if (user == null)
                throw new Exception("Invalid email or password.");

            bool isValidPassword = BC.Verify(loginDto.Password, user.Password);
            if (!isValidPassword)
                throw new Exception("Invalid email or password.");

          
            return (GenerateJwtToken(user),user);
        }

        public async void ForgotPassword(ForgotPasswordDTO forgotPasswordDto)
        {
            var user = _userRepository.GetUserByEmail(forgotPasswordDto.Email);
            if (user == null)
                throw new Exception("User not found.");

            // Generate a reset token 
            string resetToken = Guid.NewGuid().ToString();
            user.ResetToken = resetToken;
            user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1); 

            _userRepository.UpdateUser(user);

            var resetLink = $"http://localhost:4200/reset-password?token={resetToken}";

            var body = $"<h3>Reset Password</h3><p>Click below link:</p><a href='{resetLink}'>Reset Password</a>";

            await SendEmailAsync(user.Email, "Reset Password", body);
            
        }

        public bool ResetPassword(ResetPasswordDTO resetPasswordDto)
        {
            var user = _userRepository.GetUserByResetToken(resetPasswordDto.Token);
            if (user == null)
                throw new Exception("User not found.");

            if (user.ResetToken != resetPasswordDto.Token || user.ResetTokenExpiry < DateTime.UtcNow)
                throw new Exception("Invalid or expired token.");

            // Hash new password
            user.Password = BC.HashPassword(resetPasswordDto.NewPassword);
            user.ResetToken = null; 
            user.ResetTokenExpiry = null;
            user.UpdatedAt = DateTime.UtcNow;

            _userRepository.UpdateUser(user);
            return true;
        }

        public User GetUserByID(int Id)
        {
            var user = _userRepository.GetUserByID(Id);
            if (user == null)
                throw new Exception("User not found.");

            return user;
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.GivenName, user.FirstName)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }



        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpClient = new SmtpClient(_configuration["EmailSettings:SmtpServer"])
            {
                Port = int.Parse(_configuration["EmailSettings:Port"]),
                Credentials = new NetworkCredential(
                    _configuration["EmailSettings:SenderEmail"],
                    _configuration["EmailSettings:SenderPassword"]
                ),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                From = new MailAddress(_configuration["EmailSettings:SenderEmail"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mail.To.Add(toEmail);

            await smtpClient.SendMailAsync(mail);
        }
    }
}
