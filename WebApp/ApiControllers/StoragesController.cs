using System.Net.Mime;
using App.Contracts.DAL;
using PublicDTO = App.DTO.v1_0;
using Asp.Versioning;
using AutoMapper;
using Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Helpers;

namespace WebApp.ApiControllers;

/// <summary>
/// Controller for Storages
/// </summary>
/// <param name="uow">unit of work</param>
/// <param name="mapper">mapper</param>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class StoragesController(IAppUnitOfWork uow, IMapper mapper) : ControllerBase
{
    private readonly PublicDTODalMapper<App.DAL.DTO.Storage, PublicDTO.Storage> _mapper = new(mapper);

    /// <summary>
    /// Return all storages visible to current user
    /// </summary>
    /// <returns>Storage list</returns>
    [HttpGet]
    [ProducesResponseType<IEnumerable<PublicDTO.Storage>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<IEnumerable<PublicDTO.Storage>>> GetStorages()
    {
        var res = (await uow.Storages.AllAsync(User.GetUserId()))
            .Select(s => _mapper.Map(s));
        return Ok(res);
    }

    /// <summary>
    /// Return storage
    /// </summary>
    /// <param name="id">storage id</param>
    /// <returns>Storage</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType<PublicDTO.Storage>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<PublicDTO.Storage>> GetStorage(Guid id)
    {
        var storage = await uow.Storages.FindWithParentAsync(id);
        if (storage == null) return NotFound();

        return Ok(_mapper.Map(storage));
    }

    /// <summary>
    /// Modify storage
    /// </summary>
    /// <param name="id">storage id</param>
    /// <param name="storage">storage</param>
    /// <returns>No content</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> PutStorage(Guid id, PublicDTO.Storage storage)
    {
        if (id != storage.Id) return BadRequest();

        uow.Storages.Update(_mapper.Map(storage)!, User.GetUserId());
        await uow.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Add storage
    /// </summary>
    /// <param name="storage">storage</param>
    /// <returns>Storage</returns>
    [HttpPost]
    [ProducesResponseType<PublicDTO.Storage>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<PublicDTO.Storage>> PostStorage(PublicDTO.Storage storage)
    {
        uow.Storages.Add(_mapper.Map(storage)!, User.GetUserId());
        await uow.SaveChangesAsync();

        return CreatedAtAction("GetStorage", new
        {
            version = HttpContext.GetRequestedApiVersion()?.ToString(),
            id = storage.Id
        }, storage);
    }

    /// <summary>
    /// Delete storage
    /// </summary>
    /// <param name="id">storage id</param>
    /// <returns>No content</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteStorage(Guid id)
    {
        var storage = await uow.Storages.FindAsync(id);
        if (storage == null) return NotFound();

        uow.Storages.Remove(storage);
        await uow.SaveChangesAsync();

        return NoContent();
    }
}