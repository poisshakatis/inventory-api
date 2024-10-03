using System.Net.Mime;
using Data;
using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Utils;

namespace WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StoragesController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<StorageView>>> GetStorages()
    {
        var storages = await context.Storages
            .Where(s => s.AppUserId == User.GetId())
            .Select(s => new StorageView
            {
                Id = s.Id,
                Name = s.Name,
                ParentStorageId = s.ParentStorageId,
                ParentStorageName = s.ParentStorage != null ? s.ParentStorage.Name : null
            })
            .ToListAsync();

        return Ok(storages);
    }

    [HttpGet("{id:int}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStorage(int id)
    {
        var storage = await context.Storages
            .Include(s => s.ParentStorage)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (storage == null)
        {
            return NotFound();
        }

        if (storage.AppUserId != User.GetId())
        {
            return Forbid();
        }

        var res = new StorageView
        {
            Id = id,
            Name = storage.Name,
            ParentStorageId = storage.ParentStorageId,
            ParentStorageName = storage.ParentStorage?.Name
        };

        return Ok(res);
    }

    [HttpPut("{id:int}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutStorage(int id, SaveStorageRequest request)
    {
        var storage = await context.Storages.FindAsync(id);

        if (storage == null)
        {
            return NotFound();
        }
        
        if (storage.AppUserId != User.GetId())
        {
            return Forbid();
        }

        storage.Name = request.Name;
        storage.ParentStorageId = request.ParentStorageId;

        context.Update(storage);
        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PostStorage(SaveStorageRequest request)
    {
        var storage = new Domain.Storage
        {
            Name = request.Name,
            AppUserId = User.GetId(),
            ParentStorageId = request.ParentStorageId
        };
        
        context.Storages.Add(storage);
        await context.SaveChangesAsync();

        return Created();
    }
    
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteStorage(int id)
    {
        var storage = await context.Storages.FindAsync(id);

        if (storage == null)
        {
            return NotFound();
        }
        
        if (storage.AppUserId != User.GetId())
        {
            return Forbid();
        }

        context.Storages.Remove(storage);
        await context.SaveChangesAsync();

        return NoContent();
    }
}