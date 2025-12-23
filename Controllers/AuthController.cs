using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyTodoList.Data;   // DİKKAT: İsimler değişti
using MyTodoList.Models; // DİKKAT: İsimler değişti
using System;

namespace MyTodoList.Controllers // DİKKAT: MyTodoList
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                return BadRequest("Bu kullanıcı adı dolu.");

            var newUser = new User
            {
                Username = request.Username,
                Password = request.Password,
                SecurityQuestion = request.SecurityQuestion,
                SecurityAnswer = request.SecurityAnswer?.ToLower().Trim()
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Kayıt başarılı!", userId = newUser.Id });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username && u.Password == request.Password);

            if (user == null) return BadRequest("Hatalı giriş.");
            return Ok(new { message = "Giriş başarılı!", userId = user.Id });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null) return BadRequest("Kullanıcı yok.");

            if (user.SecurityQuestion != request.SecurityQuestion ||
                user.SecurityAnswer != request.SecurityAnswer?.ToLower().Trim())
            {
                return BadRequest("Güvenlik sorusu yanlış!");
            }

            user.Password = request.NewPassword;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Şifre değişti." });
        }
    }

    public class RegisterDto { public string Username { get; set; } public string Password { get; set; } public string SecurityQuestion { get; set; } public string SecurityAnswer { get; set; } }
    public class LoginDto { public string Username { get; set; } public string Password { get; set; } }
    public class ResetPasswordDto { public string Username { get; set; } public string SecurityQuestion { get; set; } public string SecurityAnswer { get; set; } public string NewPassword { get; set; } }
}