using System.Security.Claims;
using App.DTO.v1_0;
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
    public RestApiErrorResponse? ErrorResponse { get; set; }

    public AuthResult(bool success = false, T? data = default, RestApiErrorResponse? errorResponse = default)
    {
        Success = success;
        Data = data;
        ErrorResponse = errorResponse;
    }
}

public class AuthResult
{
    public bool Success { get; set; }
    public RestApiErrorResponse? ErrorResponse { get; set; }

    public AuthResult(bool success = false, RestApiErrorResponse? errorResponse = default)
    {
        Success = success;
        ErrorResponse = errorResponse;
    }
}