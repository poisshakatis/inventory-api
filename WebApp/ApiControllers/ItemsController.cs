using System.Net.Mime;
using App.Contracts.DAL;
using Microsoft.AspNetCore.Mvc;
using PublicDTO = App.DTO.v1_0;
using Asp.Versioning;
using AutoMapper;
using Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
public class ItemsController(IAppUnitOfWork uow, IMapper mapper) : ControllerBase
{
    private readonly PublicDTODalMapper<App.DAL.DTO.Item, PublicDTO.Item> _mapper = new(mapper);

    /// <summary>
    /// Return all items visible to current user
    /// </summary>
    /// <returns>Item list</returns>
    [HttpGet]
    [ProducesResponseType<IEnumerable<PublicDTO.Item>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<IEnumerable<PublicDTO.Item>>> GetItems()
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
    [ProducesResponseType<PublicDTO.Item>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<PublicDTO.Item>> GetItem(Guid id)
    {
        var storage = await uow.Items.FindWithStorageAsync(id);
        if (storage == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map(storage));
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
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> PutItem(Guid id, PublicDTO.Item item)
    {
        if (id != item.Id)
        {
            return BadRequest();
        }

        uow.Items.Update(_mapper.Map(item)!);
        await uow.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Add item
    /// </summary>
    /// <param name="item">item</param>
    /// <returns>Item</returns>
    [HttpPost]
    [ProducesResponseType<PublicDTO.Item>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<PublicDTO.Item>> PostItem(PublicDTO.Item item)
    {
        uow.Items.Add(_mapper.Map(item)!);
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

        uow.Items.Remove(item);
        await uow.SaveChangesAsync();

        return NoContent();
    }
}