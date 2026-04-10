using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManager.Business.Interface;
using TaskManager.Models.DTOs;
using TaskManager.Models.Entity;
using TaskManager.Repository.Interface;

namespace TaskManager.Business.Service
{
    public class UserBL : IUserBL
    {
        private readonly IUserRepository _userRepo;
        private readonly IConfiguration _configuration;


        public UserBL( IUserRepository userRepo, IConfiguration configuration)
        {
            _userRepo = userRepo;
            _configuration = configuration;
            
        }


        public User Register(RegisterDTO registerDto)
        { 
                var existUser = _userRepo.GetUserByUserName(registerDto.UserName);
                if (existUser != null)
                {
                    throw new Exception("User Already Exist !");
                }

                var user = new User
                {
                    UserName = registerDto.UserName,
                    Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                    Role = registerDto.Role
                };
                return _userRepo.Register(user);
                    
        }


        public (string token, User user) Login(LoginDTO loginDto)
        {
            var existUser = _userRepo.GetUserByUserName(loginDto.UserName);
            if (existUser == null)
            {
                throw new Exception("User Not Found !");
            }

            bool IsValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, existUser.Password);

            if (!IsValid)
            {
                throw new Exception("Invalid User ");
            }

            return (GenerateToken(existUser), existUser);
        }



        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role,user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

        }
    }
}
