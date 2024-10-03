using System.Net.Mime;
using Data;
using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services;
using Utils;
using WebApp.Utils;

namespace WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ItemsController(ItemService itemService, AppDbContext context) : ControllerBase
{
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<ItemView>>> GetStorages(string? query, int? limit)
    {
        var items = await itemService.GetItems(User.GetId(), query, limit);
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetItem(int id)
    {
        var item = await itemService.Find(id);

        if (item == null)
        {
            return NotFound();
        }

        if (item.Storage.AppUserId != User.GetId())
        {
            return Forbid();
        }

        var res = new ItemView
        {
            Id = item.Id,
            Name = item.Name,
            ImageName = item.ImageName,
            SerialNumber = item.SerialNumber,
            Description = item.Description,
            Category = item.Category,
            Quantity = item.Quantity,
            StorageId = item.StorageId,
            StorageName = item.Storage.Name
        };

        return Ok(res);
    }

    [HttpPut("{id:int}")]
    [Consumes(MediaTypeNames.Multipart.FormData)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutItem(int id, [FromForm] SaveItemRequest request)
    {
        var item = await itemService.Find(id);

        if (item == null)
        {
            return NotFound();
        }
        
        if (item.Storage.AppUserId != User.GetId())
        {
            return Forbid();
        }

        await itemService.Update(item, request);

        return NoContent();
    }

    [HttpPost]
    [Consumes(MediaTypeNames.Multipart.FormData)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PostItem([FromForm] SaveItemRequest request)
    {
        if (request.Image.Length == 0 || !ImageUtils.IsValidImageExtension(Path.GetExtension(request.Image.FileName)))
        {
            return BadRequest("This is not an image file! " + request.Image.FileName);
        }
        
        var item = new Domain.Item
        {
            Name = request.Name,
            ImageName = request.Image.FileName,
            SerialNumber = request.SerialNumber,
            Description = request.Description,
            Category = request.Category,
            Quantity = request.Quantity,
            StorageId = request.StorageId
        };
        
        context.Items.Add(item);
        await context.SaveChangesAsync();

        return Created();
    }
    
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteItem(int id)
    {
        var item = await itemService.Find(id);

        if (item == null)
        {
            return NotFound();
        }
        
        if (item.Storage.AppUserId != User.GetId())
        {
            return Forbid();
        }

        var imagePath = ImageUtils.GetImagePath(item.ImageName);
        System.IO.File.Delete(imagePath);

        context.Items.Remove(item);
        await context.SaveChangesAsync();

        return NoContent();
    }
}