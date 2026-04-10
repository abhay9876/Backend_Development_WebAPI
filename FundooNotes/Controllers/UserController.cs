using FundooNotes.Business.Interface;
using FundooNotes.Models.DTOs;
using FundooNotes.Models.Entity;
using Microsoft.AspNetCore.Mvc;


namespace FundooNotes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserBL _userBL;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserBL userBL, ILogger<UserController> logger)
        {
            _userBL = userBL;
            _logger = logger;
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterDTO registerDto)
        {
            _logger.LogInformation("Register attempt: {Email}", registerDto.Email);
            try
            {
                var result = _userBL.Register(registerDto);
                _logger.LogInformation("Register successful: {Email}", registerDto.Email);
                return Ok(new { success = true, message = "Registration successful", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Register failed: {Email}", registerDto.Email);
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDTO loginDto)
        {
            _logger.LogInformation("Login attempt: {Email}", loginDto.Email);

            try
            {
                var result = _userBL.Login(loginDto);

                return Ok(new
                {
                    success = true,
                    message = "Login successful",
                    token = result.token,
                    user = new
                    {
                        name = result.user.FirstName,
                        email = result.user.Email
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed: {Email}", loginDto.Email);

                return Unauthorized(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword(ForgotPasswordDTO forgotPasswordDto)
        {
            try
            {
                _userBL.ForgotPassword(forgotPasswordDto);
                
                return Ok(new { success = true, message = "Reset token generated (check email)" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword(ResetPasswordDTO resetPasswordDto)
        {
            try
            {
                var result = _userBL.ResetPassword(resetPasswordDto);
                return Ok(new { success = true, message = "Password reset successful" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }


        [HttpGet("GetUserById/{id}")]
        public IActionResult GetUserByID(int id)
        {
            try
            {
                var result = _userBL.GetUserByID(id);
                return Ok(new { success = true, data = new { result.Id, result.FirstName, result.Email } });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
