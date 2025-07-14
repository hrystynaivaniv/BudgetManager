using BudgetManager.DTOs.UserDTOs;
using BudgetManager.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpLogging;
using BudgetManager.Services;

namespace BudgetManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAuthController : ControllerBase

    {
        private readonly BudgetManagerDbContext _context;
        private readonly TokenService _tokenService;

        public UserAuthController(BudgetManagerDbContext context, TokenService tokenService)
            {
                _context = context;
            _tokenService = tokenService;
            }

            [HttpPost("register")]
            public async Task<ActionResult> Register([FromBody] UserRegisterDto dto)
            {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                    return BadRequest("This username is already taken");

                using var hmac = new HMACSHA512();

                var user = new User
                {
                    Username = dto.Username,
                    PasswordSalt = hmac.Key,
                    PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password))
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return Ok("User created successfully");
            }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] UserLoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == dto.Username);

            if (user == null)
                return Unauthorized(new { error = "Invalid username or password." });

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                    return Unauthorized(new { error = "Invalid username or password" });
            }

            var token = _tokenService.CreateToken(user);

            return Ok(new { token });
        }
        }
    }

