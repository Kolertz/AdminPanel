using System.ComponentModel.DataAnnotations;

namespace AdminPanel.Models;

public class JwtSettings
{
    [MinLength(5)]
    public required string SecretKey { get; set; }

    [Range(1, int.MaxValue)]
    public int AccessTokenExpirationMinutes { get; set; }

    [Range(1, int.MaxValue)]
    public int RefreshTokenExpirationDays { get; set; }
}
