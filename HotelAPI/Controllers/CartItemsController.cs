using Microsoft.AspNetCore.Authorization;
using HotelAPI.DTOs;
using HotelAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelAPI.Controllers;

[Route("api/cartitems")]
[ApiController]
[Authorize(Policy = "AdminOnly")]
public class CartItemsController : ControllerBase
{
    private readonly ICartItemService _service;

    public CartItemsController(ICartItemService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10, CancellationToken ct = default)
    {
        try
        {
            var (items, total) = await _service.GetAllAsync(page, pageSize, ct);
            Response.Headers["X-Total-Count"] = total.ToString(System.Globalization.CultureInfo.InvariantCulture);
            return Ok(items);
        }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var dto = await _service.GetByIdAsync(id, ct);
        if (dto is null) return NotFound();
        return Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCartItemDTO dto, CancellationToken ct)
    {
        try
        {
            var created = await _service.CreateAsync(dto, ct);
            if (created is null) return NotFound();
            return CreatedAtAction(nameof(GetById), new { id = created.IdCartItem }, created);
        }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateCartItemDTO dto, CancellationToken ct)
    {
        try
        {
            if (!await _service.UpdateAsync(id, dto, ct)) return NotFound();
            return NoContent();
        }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(int id, PatchCartItemDTO dto, CancellationToken ct)
    {
        try
        {
            if (!await _service.PatchAsync(id, dto, ct)) return NotFound();
            return NoContent();
        }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        try
        {
            if (!await _service.DeleteAsync(id, ct)) return NotFound();
            return NoContent();
        }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }
}
