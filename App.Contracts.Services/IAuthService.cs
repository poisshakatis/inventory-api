using System.Security.Claims;
using App.DTO.v1_0.Identity;

namespace App.Contracts.Services;

public interface IAuthService
{
    Task<AuthResult<JwtResponse>> RegisterAsync(RegisterInfo registrationData);
    Task<AuthResult<JwtResponse>> LoginAsync(LoginInfo loginInfo);
    Task<AuthResult<JwtResponse>> RefreshTokenAsync(TokenRefreshInfo tokenRefreshInfo);
    Task<AuthResult> LogoutAsync(LogoutInfo logout, ClaimsPrincipal user);
}

public class AuthResult<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string[]? Errors { get; set; }

    public AuthResult(bool success = false, T? data = default, params string[] errors)
    {
        Success = success;
        Data = data;
        Errors = errors;
    }
}

public class AuthResult
{
    public bool Success { get; set; }
    public string[] Errors { get; set; }

    public AuthResult(bool success = false, params string[] errors)
    {
        Success = success;
        Errors = errors;
    }
}