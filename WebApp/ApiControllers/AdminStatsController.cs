using System.Net.Mime;
using App.Contracts.DAL;
using App.Domain.Identity;
using App.DTO.v1_0;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApp.ApiControllers;

/// <summary>
/// Admin controller for statistics
/// </summary>
/// <param name="uow">unit of work</param>
/// <param name="userManager">user manager</param>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
public class AdminStatsController(IAppUnitOfWork uow, UserManager<AppUser> userManager) : ControllerBase
{
    /// <summary>
    /// Return each user's items counted
    /// </summary>
    /// <returns>user emails and items counted</returns>
    [HttpGet]
    [ProducesResponseType<List<UserItemCount>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetAllUsersWithItemCount()
    {
        var users = await userManager.Users.ToListAsync();
        var res = uow.Items.AllUsersWithItemCount(users);
        return Ok(res);
    }
}