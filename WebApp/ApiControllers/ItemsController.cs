using System.Net.Mime;
using App.Constants;
using App.Contracts.DAL;
using App.Domain.Identity;
using Microsoft.AspNetCore.Mvc;
using PublicDTO = App.DTO.v1_0;
using Asp.Versioning;
using AutoMapper;
using Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApp.Helpers;

namespace WebApp.ApiControllers;

/// <summary>
/// Controller for Items
/// </summary>
/// <param name="uow">unit of work</param>
/// <param name="mapper">mapper</param>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ItemsController(IAppUnitOfWork uow, IMapper mapper, UserManager<AppUser> userManager)
    : ControllerBase
{
    private readonly PublicDTODalMapper<App.DAL.DTO.Item, PublicDTO.ItemSend> _mapper = new(mapper);

    /// <summary>
    /// Return all items visible to current user
    /// </summary>
    /// <returns>Item list</returns>
    [HttpGet]
    [ProducesResponseType<IEnumerable<PublicDTO.ItemSend>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<IEnumerable<PublicDTO.ItemSend>>> GetItems()
    {
        var res = (await uow.Items.AllWithStorageAsync(User.GetUserId()))
            .Select(s => _mapper.Map(s));
        return Ok(res);
    }

    /// <summary>
    /// Return item
    /// </summary>
    /// <param name="id">item id</param>
    /// <returns>Item</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType<PublicDTO.ItemSend>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<PublicDTO.ItemSend>> GetItem(Guid id)
    {
        var item = await uow.Items.FindWithStorageAsync(id);
        if (item == null || !System.IO.File.Exists(item.ImagePath))
        {
            return NotFound();
        }

        return Ok(_mapper.Map(item));
    }

    /// <summary>
    /// Modify item
    /// </summary>
    /// <param name="id">item id</param>
    /// <param name="item">item</param>
    /// <returns>No content</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Consumes(MediaTypeNames.Multipart.FormData)]
    public async Task<IActionResult> PutItem(Guid id, [FromForm] PublicDTO.ItemReceive item)
    {
        if (id != item.Id)
        {
            return BadRequest();
        }

        await uow.Items.Update(item);
        await uow.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Add item
    /// </summary>
    /// <param name="item">item</param>
    /// <returns>Item</returns>
    [HttpPost]
    [ProducesResponseType<PublicDTO.ItemSend>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Consumes(MediaTypeNames.Multipart.FormData)]
    public async Task<ActionResult<PublicDTO.ItemSend>> PostItem([FromForm] PublicDTO.ItemReceive item)
    {
        if (item.Image.Length == 0 || !ImageExtensions.IsValidImageExtension(Path.GetExtension(item.Image.FileName)))
        {
            return BadRequest("This is not an image file! " + item.Image.FileName);
        }
        
        await uow.Items.Add(item);
        await uow.SaveChangesAsync();
        
        return CreatedAtAction("GetItem", new
        {
            version = HttpContext.GetRequestedApiVersion()?.ToString(),
            id = item.Id,
        }, item);
    }

    /// <summary>
    /// Delete item
    /// </summary>
    /// <param name="id">item id</param>
    /// <returns>No content</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteItem(Guid id)
    {
        var item = await uow.Items.FindAsync(id);
        if (item == null)
        {
            return NotFound();
        }
        
        System.IO.File.Delete(item.ImagePath);

        uow.Items.Remove(item);
        await uow.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Return all users with number of items per category for statistical purposes
    /// </summary>
    /// <returns>List of users with number of items per category</returns>
    [HttpGet("Statistics")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType<List<PublicDTO.UserCategoryItemCount>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetAllUsersWithCategoryItemCount()
    {
        var users = await userManager.Users.ToListAsync();
        var res = await uow.Items.AllUsersWithCategoryItemCount(users);
        return Ok(res);
    }
}