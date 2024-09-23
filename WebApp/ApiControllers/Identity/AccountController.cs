using System.Net.Mime;
using App.Contracts.Services;
using App.DTO.v1_0;
using App.DTO.v1_0.Identity;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.ApiControllers.Identity;

[ApiVersion("1.0")]
[ApiController]
[Route("/api/v{version:apiVersion}/identity/[controller]/[action]")]
public class AccountController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IAuthService _authService;

    public AccountController(IConfiguration configuration, IAuthService authService)
    {
        _configuration = configuration;
        _authService = authService;
    }

    /// <summary>
    /// Register new local user into app.
    /// </summary>
    /// <param name="registrationData">Username and password. And personal details.</param>
    /// <returns>JWTResponse - jwt and refresh token</returns>
    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType<JwtResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<RestApiErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<JwtResponse>> Register([FromBody] RegisterInfo registrationData)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.RegisterAsync(registrationData);

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result.Errors);
    }

    /// <summary>
    /// Log user in
    /// </summary>
    /// <param name="loginInfo">user info</param>
    /// <returns>JwtResponse with jwt and refreshtoken</returns>
    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(JwtResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RestApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<string>> Login([FromBody] LoginInfo loginInfo)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.LoginAsync(loginInfo);

        if (result.Success)
        {
            return Ok(result.Data);
        }

        return BadRequest(result.Errors);
    }

    /// <summary>
    /// Refresh token
    /// </summary>
    /// <param name="tokenRefreshInfo">jwt and refreshtoken</param>
    /// <returns>JwtResponse with jwt and refreshtoken</returns>
    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(JwtResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RestApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(RestApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JwtResponse>> RefreshTokenData([FromBody] TokenRefreshInfo tokenRefreshInfo)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.RefreshTokenAsync(tokenRefreshInfo);

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result.Errors);
    }

    /// <summary>
    /// Log user out
    /// </summary>
    /// <param name="logout">refresh token</param>
    /// <returns>Deleted tokens count</returns>
    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(JwtResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RestApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Logout(
        [FromBody] LogoutInfo logout)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.LogoutAsync(logout, User);

        if (result.Success)
        {
            return Ok();
        }

        return BadRequest(result.Errors);
    }
}