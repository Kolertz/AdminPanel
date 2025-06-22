using AdminPanel.Models.Dtos;
using AdminPanel.Models.Requests;

namespace AdminPanel.Interfaces;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginRequestBody model);
    Task<AuthResponse?> RefreshTokenAsync(string refreshToken);
}
