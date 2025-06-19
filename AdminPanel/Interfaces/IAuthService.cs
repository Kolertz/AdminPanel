using AdminPanel.Models;

namespace AdminPanel.Interfaces;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginRequestBody model);
    Task<AuthResponse?> RefreshTokenAsync(string refreshToken);
}
