using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Business.Interface;
using TaskManager.Models.DTOs;
using TaskManager.Models.Entity;
using RouteAttribute = Microsoft.AspNetCore.Components.RouteAttribute;

namespace TaskManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserBL _userBL;

        public UserController(IUserBL userBL)
        {
            _userBL = userBL;
        }
        [HttpPost("register")]
        public IActionResult Register(RegisterDTO registerDto)
        {
            try
            {
                var result = _userBL.Register(registerDto);
                return Ok(new { success = true, data = result, message = "User registered successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Error while Register", error = ex.Message });
            }
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDTO loginDto)
        {
            try
            {
                var (token, user) = _userBL.Login(loginDto);

                return Ok(new
                {
                    success = true,
                    token = token,
                    role = user.Role,
                    message = "Login successful"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Error while Login", error = ex.Message });
            }


        }
    }
}
