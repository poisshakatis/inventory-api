using System.Net.Mime;
using System.Security.Claims;
using Domain;
using Domain.Identity;
using DTO.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Services;

namespace WebApp.Controllers.Identity;

[ApiController]
[Route("/api/identity/[controller]/[action]")]
public class AccountController(
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager,
    JwtTokenService jwtTokenService,
    RefreshTokenService refreshTokenService,
    IOptions<JwtSettings> jwtSettings)
    : ControllerBase
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var user = new AppUser
        {
            Email = request.Email,
            UserName = request.Email
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return BadRequest(new { Message = result.Errors.First().Description });
        }

        var claimsPrincipal = await signInManager.CreateUserPrincipalAsync(user);
        var accessToken = jwtTokenService.GenerateAccessToken(claimsPrincipal.Claims);
        var refreshToken = JwtTokenService.GenerateRefreshToken();

        return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
    }

    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            return Unauthorized(new { Message = "Invalid email or password." });
        }

        var claimsPrincipal = await signInManager.CreateUserPrincipalAsync(user);
        var accessToken = jwtTokenService.GenerateAccessToken(claimsPrincipal.Claims);
        var token = JwtTokenService.GenerateRefreshToken();

        var refreshToken = new RefreshToken
        {
            Token = token,
            Username = request.Email,
            ExpirationDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays)
        };

        return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
    }

    [HttpPost]
    [Authorize]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult Refresh([FromBody] TokenRequest request)
    {
        var storedRefreshToken = refreshTokenService.GetStoredRefreshToken(request.RefreshToken);
        if (storedRefreshToken is null || storedRefreshToken.ExpirationDate < DateTime.UtcNow)
        {
            return Unauthorized(new { Message = "Invalid or expired refresh token." });
        }

        var userName = User.FindFirstValue(ClaimTypes.Name);
        if (storedRefreshToken.Username != userName)
        {
            return Forbid();
        }

        var newAccessToken = jwtTokenService.GenerateAccessToken(User.Claims);
        var newRefreshToken = JwtTokenService.GenerateRefreshToken();

        storedRefreshToken.Token = newRefreshToken;
        storedRefreshToken.ExpirationDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);
        refreshTokenService.SaveRefreshToken(storedRefreshToken);

        return Ok(new { AccessToken = newAccessToken, RefreshToken = newRefreshToken });
    }

    [HttpPost]
    [Authorize]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult Logout([FromBody] TokenRequest request)
    {
        var storedRefreshToken = refreshTokenService.GetStoredRefreshToken(request.RefreshToken);
        var userName = User.FindFirstValue(ClaimTypes.Name);

        if (storedRefreshToken is not null && storedRefreshToken.Username != userName)
        {
            return Forbid();
        }

        if (storedRefreshToken is not null)
        {
            refreshTokenService.RemoveRefreshToken(storedRefreshToken);
        }

        return NoContent();
    }
}