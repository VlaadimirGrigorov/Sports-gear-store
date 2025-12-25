using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SportsGearStore.Data;
using SportsGearStore.DTO;
using SportsGearStore.Helpers;
using SportsGearStore.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SportsGearStore.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly SportsGearStoreDbContext _context;
    private readonly JwtSettings _jwtSettings;

    public AuthController(SportsGearStoreDbContext context, IOptions<JwtSettings> jwtSettings)
    {
        _context = context;
        _jwtSettings = jwtSettings.Value;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginDTO dto)
    {
        // Търсим user по username (или email) – тук показвам пример само с username
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
        if (user == null)
        {
            return Unauthorized("Invalid username.");
        }

        // Проверяваме дали подадената парола съвпада с хеша в базата
        var incomingHash = GenericHelpers.ComputeSha256Hash(dto.Password);
        if (incomingHash != user.Password)
        {
            return Unauthorized("Invalid password.");
        }

        // Ако всичко е ОК – генерираме JWT
        var token = GenerateJwtToken(user);

        return Ok(new { Token = token });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterDTO dto)
    {
        var email = await _context.Users.FirstOrDefaultAsync(c => c.Useremail == dto.Email);
        if (email != null)
        {
            return Conflict("Email already exists!");
        }

        var username = await _context.Users.FirstOrDefaultAsync(c => c.Username == dto.Username);
        if (username != null)
        {
            return Conflict("Username already exists!");
        }

        // Хешираме паролата
        string passwordHash = GenericHelpers.ComputeSha256Hash(dto.Password);

        var user = new User
        {
            Username = dto.Username,
            Useremail = dto.Email,
            Password = passwordHash
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Можем да върнем само базова информация за новия user (без парола)
        return CreatedAtAction(nameof(Register), new { id = user.Id }, new { user.Id, user.Username, user.Useremail });
    }

    private string GenerateJwtToken(User user)
    {
        var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecurityKey));

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.TokenDurationMinutes),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
