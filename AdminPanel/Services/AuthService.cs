using AdminPanel.Interfaces;
using AdminPanel.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AdminPanel.Services;

public class AuthService(AppDbContext db, IOptions<JwtSettings> options) : IAuthService
{
    private readonly AppDbContext _db = db;
    private readonly JwtSettings _jwtSettings = options.Value;

    public async Task<AuthResponse?> LoginAsync(LoginRequestBody model)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            return null;

        var token = GenerateJwtToken(user);

        var refreshToken = new RefreshToken
        {
            Token = Guid.NewGuid().ToString(),
            UserId = user.Id,
            Expires = DateTime.UtcNow.AddDays(7) // RefreshToken живет 7 дней
        };

        _db.RefreshTokens.Add(refreshToken);
        await _db.SaveChangesAsync();

        return new()
        {
            Token = token,
            RefreshToken = refreshToken.Token
        };
    }

    public async Task<AuthResponse?> RefreshTokenAsync(string refreshToken)
    {
        var storedToken = await _db.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (storedToken == null || storedToken.Expires < DateTime.UtcNow || storedToken.User == null)
            return null;

        var newToken = GenerateJwtToken(storedToken.User);

        var newRefreshToken = new RefreshToken
        {
            Token = Guid.NewGuid().ToString(),
            UserId = storedToken.UserId,
            Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays)
        };

        _db.RefreshTokens.Remove(storedToken);
        _db.RefreshTokens.Add(newRefreshToken);
        await _db.SaveChangesAsync();

        return new()
        {
            Token = newToken,
            RefreshToken = newRefreshToken.Token
        };
    }

    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.SecretKey)),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}